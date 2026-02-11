using System.Collections.Concurrent;

namespace Bam.Data.Objects;

public class ObjectDataSearcher : IObjectDataSearcher
{
    public ObjectDataSearcher(IObjectDataSearchIndexer searchIndexer, IObjectDataReader reader)
    {
        this.SearchIndexer = searchIndexer;
        this.Reader = reader;
    }

    private IObjectDataSearchIndexer SearchIndexer { get; }
    private IObjectDataReader Reader { get; }

    public async Task<IObjectDataSearchResult> SearchAsync(IObjectDataSearch dataSearch)
    {
        try
        {
            Type type = dataSearch.Type;
            ObjectDataSearchCriterion[] criteria = dataSearch.Criteria.ToArray();

            if (criteria.Length == 0)
            {
                return new ObjectDataSearchResult
                {
                    Success = true,
                    Results = Enumerable.Empty<IObjectData>(),
                    TotalCount = 0
                };
            }

            // 1. Parallel criteria lookup: each criterion hits its own index file
            ConcurrentBag<HashSet<string>> perCriterionKeys = new();

            Parallel.ForEach(criteria, criterion =>
            {
                string valueHash = ObjectDataSearchIndexer.ComputeValueHash(
                    EncodeValue(criterion.Value));
                IEnumerable<IObjectDataKey> keys = SearchIndexer
                    .LookupAsync(type, criterion.PropertyName, valueHash)
                    .GetAwaiter().GetResult();
                perCriterionKeys.Add(new HashSet<string>(keys.Select(k => k.Key)));
            });

            // 2. Intersect all sets (AND semantics)
            HashSet<string> matchingKeys = null;
            foreach (HashSet<string> keySet in perCriterionKeys)
            {
                if (matchingKeys == null)
                {
                    matchingKeys = keySet;
                }
                else
                {
                    matchingKeys.IntersectWith(keySet);
                }
            }

            if (matchingKeys == null || matchingKeys.Count == 0)
            {
                return new ObjectDataSearchResult
                {
                    Success = true,
                    Results = Enumerable.Empty<IObjectData>(),
                    TotalCount = 0
                };
            }

            // 3. Parallel object loading
            ConcurrentBag<IObjectData> results = new();

            Parallel.ForEach(matchingKeys, hexKey =>
            {
                IObjectDataKey key = new ObjectDataKey
                {
                    TypeDescriptor = new TypeDescriptor(type),
                    Key = hexKey
                };
                IObjectDataReadResult readResult = Reader
                    .ReadObjectDataAsync(key).GetAwaiter().GetResult();
                if (readResult?.ObjectData != null)
                {
                    results.Add(readResult.ObjectData);
                }
            });

            return new ObjectDataSearchResult
            {
                Success = true,
                Results = results.ToList(),
                TotalCount = results.Count
            };
        }
        catch (Exception ex)
        {
            return new ObjectDataSearchResult
            {
                Success = false,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod
                    ? ex.GetBaseException().Message
                    : ex.GetMessageAndStackTrace(),
                Results = Enumerable.Empty<IObjectData>(),
                TotalCount = 0
            };
        }
    }

    private static string EncodeValue(object value)
    {
        if (value == null)
        {
            return "null";
        }

        return JsonObjectDataEncoder.Default.Encode(value).ToString();
    }
}
