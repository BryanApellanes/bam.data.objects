using System.Reflection;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

/// <summary>
/// File-system-based repository implementation that provides CRUD, query, and batch operations using object data storage with composite key indexing and search indexing.
/// </summary>
public class ObjectDataRepository : AsyncRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataRepository"/> class with all required dependencies.
    /// </summary>
    /// <param name="factory">The factory used to create object data wrappers.</param>
    /// <param name="writer">The writer used to persist object data.</param>
    /// <param name="indexer">The indexer used to maintain ID and UUID indices.</param>
    /// <param name="deleter">The deleter used to remove object data and indices.</param>
    /// <param name="archiver">The archiver used for archive operations.</param>
    /// <param name="reader">The reader used to load object data from storage.</param>
    /// <param name="searcher">The searcher used for multi-criteria search operations.</param>
    /// <param name="searchIndexer">The search indexer used to maintain property value search indices.</param>
    /// <param name="compositeKeyCalculator">The composite key calculator used to compute IDs.</param>
    public ObjectDataRepository(IObjectDataFactory factory, IObjectDataWriter writer, IObjectDataIndexer indexer, IObjectDataDeleter deleter, IObjectDataArchiver archiver, IObjectDataReader reader, IObjectDataSearcher searcher, IObjectDataSearchIndexer searchIndexer, ICompositeKeyCalculator compositeKeyCalculator)
    {
        this.Factory = factory;
        this.Writer = writer;
        this.Indexer = indexer;
        this.Deleter = deleter;
        this.Archiver = archiver;
        this.Reader = reader;
        this.Searcher = searcher;
        this.SearchIndexer = searchIndexer;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    protected IObjectDataFactory Factory { get; }
    protected IObjectDataWriter Writer { get; }
    protected IObjectDataIndexer Indexer { get; }
    protected IObjectDataDeleter Deleter { get; }
    protected IObjectDataArchiver Archiver { get; }
    protected IObjectDataReader Reader { get; }
    protected IObjectDataSearcher Searcher { get; }
    protected IObjectDataSearchIndexer SearchIndexer { get; }
    protected ICompositeKeyCalculator CompositeKeyCalculator { get; }

    /// <inheritdoc />
    public override T Create<T>(T toCreate)
    {
        IObjectData objectData = Factory.GetObjectData(toCreate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(typeof(T));
        keyProp?.SetValue(toCreate, id);

        Task<IObjectDataWriteResult> writeTask = Writer.WriteAsync(objectData);
        writeTask.GetAwaiter().GetResult();

        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();
        SearchIndexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toCreate;
    }

    /// <inheritdoc />
    public override object Create(object toCreate)
    {
        IObjectData objectData = Factory.GetObjectData(toCreate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(toCreate.GetType());
        keyProp?.SetValue(toCreate, id);

        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();
        SearchIndexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toCreate;
    }

    /// <inheritdoc />
    public override object Create(Type type, object toCreate)
    {
        return Create(toCreate);
    }

    /// <inheritdoc />
    public override T Retrieve<T>(ulong id)
    {
        IObjectDataKey key = Indexer.LookupAsync(typeof(T), id).GetAwaiter().GetResult()!;
        if (key == null)
        {
            return default!;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        if (result?.ObjectData?.Data == null)
        {
            return default!;
        }

        return (T)result.ObjectData.Data;
    }

    /// <inheritdoc />
    public override T Retrieve<T>(int id)
    {
        return Retrieve<T>((ulong)id);
    }

    /// <inheritdoc />
    public override T Retrieve<T>(long id)
    {
        return Retrieve<T>((ulong)id);
    }

    /// <inheritdoc />
    public override T Retrieve<T>(string uuid)
    {
        IObjectDataKey key = Indexer.LookupByUuidAsync(typeof(T), uuid).GetAwaiter().GetResult()!;
        if (key == null)
        {
            return default!;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        if (result?.ObjectData?.Data == null)
        {
            return default!;
        }

        return (T)result.ObjectData.Data;
    }

    /// <inheritdoc />
    public override object Retrieve(Type objectType, long id)
    {
        return Retrieve(objectType, (ulong)id);
    }

    /// <inheritdoc />
    public override object Retrieve(Type objectType, ulong id)
    {
        IObjectDataKey key = Indexer.LookupAsync(objectType, id).GetAwaiter().GetResult()!;
        if (key == null)
        {
            return null!;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        return result?.ObjectData?.Data!;
    }

    /// <inheritdoc />
    public override object Retrieve(Type objectType, string uuid)
    {
        IObjectDataKey key = Indexer.LookupByUuidAsync(objectType, uuid).GetAwaiter().GetResult()!;
        if (key == null)
        {
            return null!;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        return result?.ObjectData?.Data!;
    }

    /// <inheritdoc />
    public override T Update<T>(T toUpdate)
    {
        IObjectData objectData = Factory.GetObjectData(toUpdate!);
        IObjectData? previous = LoadStoredState(objectData);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(typeof(T));
        keyProp?.SetValue(toUpdate, id);

        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();
        SearchIndexer.ReindexAsync(previous, objectData).GetAwaiter().GetResult();

        return toUpdate;
    }

    /// <inheritdoc />
    public override object Update(object toUpdate)
    {
        IObjectData objectData = Factory.GetObjectData(toUpdate);
        IObjectData? previous = LoadStoredState(objectData);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(toUpdate.GetType());
        keyProp?.SetValue(toUpdate, id);

        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();
        SearchIndexer.ReindexAsync(previous, objectData).GetAwaiter().GetResult();

        return toUpdate;
    }

    /// <summary>
    /// Loads the currently stored state of the object identified by <paramref name="objectData"/>'s
    /// key, wrapped fresh via the factory so its properties and key resolve from the stored
    /// instance.  Returns null when no stored state exists.  Must be called BEFORE the new state
    /// is written — the prior property values are what <see cref="IObjectDataSearchIndexer.ReindexAsync"/>
    /// needs to remove superseded search-index entries.
    /// </summary>
    private IObjectData? LoadStoredState(IObjectData objectData)
    {
        IObjectDataReadResult readResult = Reader.ReadObjectDataAsync(objectData.GetObjectKey()).GetAwaiter().GetResult();
        if (readResult?.ObjectData?.Data == null)
        {
            return null;
        }

        return Factory.GetObjectData(readResult.ObjectData.Data);
    }

    /// <inheritdoc />
    public override object Update(Type type, object toUpdate)
    {
        return Update(toUpdate);
    }

    /// <inheritdoc />
    public override bool Delete<T>(T toDelete)
    {
        IObjectData objectData = Factory.GetObjectData(toDelete!);
        SearchIndexer.RemoveAsync(objectData).GetAwaiter().GetResult();
        IObjectDataDeleteResult result = Deleter.DeleteAsync(objectData).GetAwaiter().GetResult();
        return result.Success;
    }

    /// <inheritdoc />
    public override bool Delete(object toDelete)
    {
        IObjectData objectData = Factory.GetObjectData(toDelete);
        SearchIndexer.RemoveAsync(objectData).GetAwaiter().GetResult();
        IObjectDataDeleteResult result = Deleter.DeleteAsync(objectData).GetAwaiter().GetResult();
        return result.Success;
    }

    /// <inheritdoc />
    public override bool Delete(Type type, object toDelete)
    {
        return Delete(toDelete);
    }

    /// <inheritdoc />
    public override IEnumerable<T> RetrieveAll<T>()
    {
        IEnumerable<IObjectDataKey> keys = Indexer.GetAllKeysAsync(typeof(T)).GetAwaiter().GetResult();
        List<T> results = new List<T>();
        foreach (IObjectDataKey key in keys)
        {
            IObjectDataReadResult readResult = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
            if (readResult?.ObjectData?.Data is T typedData)
            {
                results.Add(typedData);
            }
        }

        return results;
    }

    /// <inheritdoc />
    public override IEnumerable<object> RetrieveAll(Type type)
    {
        IEnumerable<IObjectDataKey> keys = Indexer.GetAllKeysAsync(type).GetAwaiter().GetResult();
        List<object> results = new List<object>();
        foreach (IObjectDataKey key in keys)
        {
            IObjectDataReadResult readResult = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
            if (readResult?.ObjectData?.Data != null)
            {
                results.Add(readResult.ObjectData.Data);
            }
        }

        return results;
    }

    /// <inheritdoc />
    public override void BatchRetrieveAll(Type dtoOrPocoType, int batchSize, Action<IEnumerable<object>> processor)
    {
        IEnumerable<object> all = RetrieveAll(dtoOrPocoType);
        List<object> batch = new List<object>();
        foreach (object item in all)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                processor(batch);
                batch = new List<object>();
            }
        }

        if (batch.Count > 0)
        {
            processor(batch);
        }
    }

    /// <inheritdoc />
    public override IEnumerable<T> Query<T>(Func<T, bool> query)
    {
        return RetrieveAll<T>().Where(query);
    }

    /// <inheritdoc />
    public override IEnumerable<object> Query(Type type, Func<object, bool> predicate)
    {
        return RetrieveAll(type).Where(predicate);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Equality criteria are resolved through the search index (hash lookup + verify-on-read)
    /// instead of a full scan when the index is usable; see <see cref="TrySearchIndex"/> for the
    /// fallback conditions.
    /// </remarks>
    public override IEnumerable<T> Query<T>(Dictionary<string, object> queryParameters)
    {
        IEnumerable<object>? indexed = TrySearchIndex(typeof(T), queryParameters);
        if (indexed != null)
        {
            return indexed.OfType<T>().ToList();
        }

        return RetrieveAll<T>().Where(item => MatchesQueryParameters(item, queryParameters));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Equality criteria are resolved through the search index when usable; see <see cref="TrySearchIndex"/>.
    /// </remarks>
    public override IEnumerable<object> Query(Type type, Dictionary<string, object> queryParameters)
    {
        IEnumerable<object>? indexed = TrySearchIndex(type, queryParameters);
        if (indexed != null)
        {
            return indexed.ToList();
        }

        return RetrieveAll(type).Where(item => MatchesQueryParameters(item, queryParameters));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Parameter-token equality filters are resolved through the search index when usable; see
    /// <see cref="TrySearchIndex"/>.
    /// </remarks>
    public override IEnumerable<T> Query<T>(IQueryFilter query)
    {
        IEnumerable<object>? indexed = TrySearchIndex(typeof(T), GetEqualityCriteria(query));
        if (indexed != null)
        {
            return indexed.OfType<T>().ToList();
        }

        return RetrieveAll<T>().Where(item => MatchesQueryFilter(item, query));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Parameter-token equality filters are resolved through the search index when usable; see
    /// <see cref="TrySearchIndex"/>.
    /// </remarks>
    public override IEnumerable<object> Query(Type type, IQueryFilter query)
    {
        IEnumerable<object>? indexed = TrySearchIndex(type, GetEqualityCriteria(query));
        if (indexed != null)
        {
            return indexed.ToList();
        }

        return RetrieveAll(type).Where(item => MatchesQueryFilter(item, query));
    }

    /// <summary>
    /// Attempts to resolve equality criteria through the search index, returning null when the
    /// caller must fall back to a full scan.  Fallback conditions: no criteria (an index search
    /// with zero criteria returns nothing, while the scan path matches everything); any null
    /// criterion value (null-valued properties are never indexed); no index directory for the
    /// type (a legacy store written before search indexing — an empty lookup there means
    /// "unknown", not "no matches"); or a failed search.  Results are verified against live
    /// property values by <see cref="ObjectDataSearcher"/>, so a hit list is value-identical to
    /// what the scan path would produce.
    /// </summary>
    private IEnumerable<object>? TrySearchIndex(Type type, IEnumerable<KeyValuePair<string, object>> criteria)
    {
        List<KeyValuePair<string, object>> criteriaList = criteria.ToList();
        if (criteriaList.Count == 0
            || criteriaList.Any(criterion => criterion.Value == null)
            || !SearchIndexer.HasIndex(type))
        {
            return null;
        }

        ObjectDataSearch search = new ObjectDataSearch(type);
        foreach (KeyValuePair<string, object> criterion in criteriaList)
        {
            search.Where(criterion.Key, criterion.Value);
        }

        IObjectDataSearchResult searchResult = Searcher.SearchAsync(search).GetAwaiter().GetResult();
        if (!searchResult.Success)
        {
            return null;
        }

        return searchResult.Results
            .Where(objectData => objectData.Data != null)
            .Select(objectData => objectData.Data);
    }

    /// <summary>
    /// Extracts the (column, value) equality pairs from a query filter's parameter tokens,
    /// mirroring <see cref="MatchesQueryFilter"/>: parameter tokens with a null column name and
    /// non-parameter tokens are ignored.
    /// </summary>
    private static IEnumerable<KeyValuePair<string, object>> GetEqualityCriteria(IQueryFilter query)
    {
        foreach (IFilterToken token in query.Filters)
        {
            if (token is IParameterInfo parameterInfo && parameterInfo.ColumnName != null)
            {
                yield return new KeyValuePair<string, object>(parameterInfo.ColumnName, parameterInfo.Value!);
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<object> Query(string propertyName, object propertyValue)
    {
        if (DefaultType == null)
        {
            return Enumerable.Empty<object>();
        }

        return RetrieveAll(DefaultType).Where(item =>
        {
            PropertyInfo prop = item.GetType().GetProperty(propertyName)!;
            if (prop == null)
            {
                return false;
            }

            object value = prop.GetValue(item)!;
            return Equals(value, propertyValue);
        });
    }

    private static bool MatchesQueryParameters(object item, Dictionary<string, object> queryParameters)
    {
        foreach (KeyValuePair<string, object> param in queryParameters)
        {
            PropertyInfo prop = item.GetType().GetProperty(param.Key)!;
            if (prop == null)
            {
                return false;
            }

            object value = prop.GetValue(item)!;
            if (!Equals(value, param.Value))
            {
                return false;
            }
        }

        return true;
    }

    private static bool MatchesQueryFilter(object item, IQueryFilter query)
    {
        foreach (IFilterToken token in query.Filters)
        {
            if (token is IParameterInfo parameterInfo)
            {
                string columnName = parameterInfo.ColumnName!;
                object filterValue = parameterInfo.Value!;
                if (columnName == null)
                {
                    continue;
                }

                PropertyInfo prop = item.GetType().GetProperty(columnName)!;
                if (prop == null)
                {
                    return false;
                }

                object itemValue = prop.GetValue(item)!;
                if (!Equals(itemValue, filterValue))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
