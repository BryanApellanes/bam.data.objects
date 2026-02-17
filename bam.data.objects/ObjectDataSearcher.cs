using System.Collections.Concurrent;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataSearcher"/> that combines hash-based index lookups for equality criteria with in-memory scanning for other operators.
/// </summary>
public class ObjectDataSearcher : IObjectDataSearcher
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataSearcher"/> class.
    /// </summary>
    /// <param name="searchIndexer">The search indexer used for hash-based property value lookups.</param>
    /// <param name="reader">The reader used to load object data from storage.</param>
    /// <param name="indexer">The indexer used to enumerate all keys for a type.</param>
    public ObjectDataSearcher(IObjectDataSearchIndexer searchIndexer, IObjectDataReader reader, IObjectDataIndexer indexer)
    {
        this.SearchIndexer = searchIndexer;
        this.Reader = reader;
        this.Indexer = indexer;
    }

    private IObjectDataSearchIndexer SearchIndexer { get; }
    private IObjectDataReader Reader { get; }
    private IObjectDataIndexer Indexer { get; }

    /// <inheritdoc />
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

            ObjectDataSearchCriterion[] equalsCriteria = criteria.Where(c => c.Operator == SearchOperator.Equals).ToArray();
            ObjectDataSearchCriterion[] nonEqualsCriteria = criteria.Where(c => c.Operator != SearchOperator.Equals).ToArray();

            List<HashSet<string>> allKeySets = new();

            // 1. Hash-based lookup for Equals criteria
            if (equalsCriteria.Length > 0)
            {
                ConcurrentBag<HashSet<string>> perCriterionKeys = new();

                Parallel.ForEach(equalsCriteria, criterion =>
                {
                    string valueHash = ObjectDataSearchIndexer.ComputeValueHash(
                        EncodeValue(criterion.Value));
                    IEnumerable<IObjectDataKey> keys = SearchIndexer
                        .LookupAsync(type, criterion.PropertyName, valueHash)
                        .GetAwaiter().GetResult();
                    perCriterionKeys.Add(new HashSet<string>(keys.Select(k => k.Key!)));
                });

                foreach (HashSet<string> keySet in perCriterionKeys)
                {
                    allKeySets.Add(keySet);
                }
            }

            // 2. In-memory scan for non-Equals criteria
            if (nonEqualsCriteria.Length > 0)
            {
                IEnumerable<IObjectDataKey> allKeys = await Indexer.GetAllKeysAsync(type);
                IObjectDataKey[] allKeysArray = allKeys.ToArray();

                ConcurrentDictionary<string, IObjectData> loadedObjects = new();

                Parallel.ForEach(allKeysArray, objectKey =>
                {
                    IObjectDataReadResult readResult = Reader
                        .ReadObjectDataAsync(objectKey).GetAwaiter().GetResult();
                    if (readResult?.ObjectData != null)
                    {
                        loadedObjects[objectKey.Key!] = readResult.ObjectData;
                    }
                });

                foreach (ObjectDataSearchCriterion criterion in nonEqualsCriteria)
                {
                    HashSet<string> matchingKeys = new();
                    string searchValue = criterion.Value?.ToString() ?? string.Empty;

                    foreach (KeyValuePair<string, IObjectData> kvp in loadedObjects)
                    {
                        object data = kvp.Value.Data;
                        if (data == null) continue;

                        System.Reflection.PropertyInfo prop = data.GetType().GetProperty(criterion.PropertyName)!;
                        if (prop == null) continue;

                        object propValue = prop.GetValue(data)!;
                        string propertyValue = propValue?.ToString() ?? string.Empty;

                        if (MatchesOperator(propertyValue, searchValue, criterion.Operator))
                        {
                            matchingKeys.Add(kvp.Key);
                        }
                    }

                    allKeySets.Add(matchingKeys);
                }
            }

            // 3. Intersect all sets (AND semantics)
            HashSet<string> matchingKeysResult = null!;
            foreach (HashSet<string> keySet in allKeySets)
            {
                if (matchingKeysResult == null)
                {
                    matchingKeysResult = keySet;
                }
                else
                {
                    matchingKeysResult.IntersectWith(keySet);
                }
            }

            if (matchingKeysResult == null || matchingKeysResult.Count == 0)
            {
                return new ObjectDataSearchResult
                {
                    Success = true,
                    Results = Enumerable.Empty<IObjectData>(),
                    TotalCount = 0
                };
            }

            // 4. Parallel object loading
            ConcurrentBag<IObjectData> results = new();

            Parallel.ForEach(matchingKeysResult, hexKey =>
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

    private static bool MatchesOperator(string propertyValue, string searchValue, SearchOperator op)
    {
        return op switch
        {
            SearchOperator.StartsWith => propertyValue.StartsWith(searchValue, StringComparison.OrdinalIgnoreCase),
            SearchOperator.EndsWith => propertyValue.EndsWith(searchValue, StringComparison.OrdinalIgnoreCase),
            SearchOperator.Contains => propertyValue.Contains(searchValue, StringComparison.OrdinalIgnoreCase),
            SearchOperator.DoesntStartWith => !propertyValue.StartsWith(searchValue, StringComparison.OrdinalIgnoreCase),
            SearchOperator.DoesntEndWith => !propertyValue.EndsWith(searchValue, StringComparison.OrdinalIgnoreCase),
            SearchOperator.DoesntContain => !propertyValue.Contains(searchValue, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static string EncodeValue(object value)
    {
        if (value == null)
        {
            return "null";
        }

        return JsonObjectDataEncoder.Default.Encode(value).ToString()!;
    }
}
