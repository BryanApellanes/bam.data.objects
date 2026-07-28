using System.Reflection;
using Bam.Data.Repositories;
using Bam.Logging;

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

        // Search-index before write: a crash between the two leaves a stale entry readers
        // verify-filter (harmless), instead of a stored row invisible to indexed queries.
        SearchIndexer.IndexAsync(objectData).GetAwaiter().GetResult();
        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toCreate;
    }

    /// <inheritdoc />
    public override object Create(object toCreate)
    {
        IObjectData objectData = Factory.GetObjectData(toCreate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(toCreate.GetType());
        keyProp?.SetValue(toCreate, id);

        // Search-index before write: a crash between the two leaves a stale entry readers
        // verify-filter (harmless), instead of a stored row invisible to indexed queries.
        SearchIndexer.IndexAsync(objectData).GetAwaiter().GetResult();
        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();

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
    /// criterion value (null-valued properties are never indexed); a criterion property that is
    /// missing on the type, is not index-enumerable (its type maps to
    /// <c>DataTypes.Default</c> — Guid, enums, float/double, nullable primitives, complex
    /// types — mirroring the gate <see cref="ObjectData"/> applies at index time), has no
    /// per-property index directory (never indexed, e.g. newly added), or whose value cannot
    /// be coerced to the property's declared type; no index directory for the type (a legacy
    /// store written before search indexing); or a failed search.
    /// <para>
    /// The index is AUTHORITATIVE for its results: positives are verified against live property
    /// values by <see cref="ObjectDataSearcher"/> (never wrong), but an empty result reflects
    /// the index, not a scan — rows persisted by a process that crashed before indexing, or in
    /// a store never migrated via <see cref="IObjectDataSearchIndexer.RebuildAsync(Type)"/>,
    /// are not found.  Stores adopting search indexing MUST run <c>RebuildAsync</c> once per
    /// type; see the consistency contract on <see cref="IObjectDataSearchIndexer"/>.
    /// </para>
    /// </summary>
    private IEnumerable<object>? TrySearchIndex(Type type, IEnumerable<KeyValuePair<string, object>> criteria)
    {
        List<KeyValuePair<string, object>> criteriaList = criteria.ToList();
        if (criteriaList.Count == 0
            || criteriaList.Any(criterion => criterion.Value == null))
        {
            return null;
        }

        if (!SearchIndexer.HasIndex(type))
        {
            Log.Warn(
                "No search index exists for type '{0}'; equality query is falling back to a full scan. If this store predates search indexing, run IObjectDataSearchIndexer.RebuildAsync for the type.",
                type.FullName!);
            return null;
        }

        ObjectDataSearch search = new ObjectDataSearch(type);
        foreach (KeyValuePair<string, object> criterion in criteriaList)
        {
            PropertyInfo? property = type.GetProperty(criterion.Key);
            if (property == null)
            {
                return null;
            }

            // Mirror the index-time gate: property types mapping to DataTypes.Default are never
            // indexed, so an indexed lookup on them is a guaranteed false empty.
            if (DataTypeTranslator.Default.EnumFromType(property.PropertyType) == DataTypes.Default)
            {
                return null;
            }

            // Never-indexed property (no object has carried a non-null value for it, e.g. a
            // property newly added to the type) — its absence from the index means "unknown".
            if (!SearchIndexer.HasIndex(type, criterion.Key))
            {
                return null;
            }

            if (!TryCoerceToPropertyType(criterion.Value, property.PropertyType, out object coercedValue))
            {
                return null;
            }

            search.Where(criterion.Key, coercedValue);
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
    /// Coerces a criterion value to the property's declared type (unwrapping Nullable) so the
    /// indexed hash, verify-on-read, and scan comparisons all evaluate the same typed value —
    /// an <c>int 5</c> criterion matches a <c>long</c> property on every path.  Returns false
    /// when the value cannot represent the property's type.
    /// </summary>
    private static bool TryCoerceToPropertyType(object value, Type propertyType, out object coercedValue)
    {
        coercedValue = value;
        Type targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (targetType.IsInstanceOfType(value))
        {
            return true;
        }

        try
        {
            coercedValue = Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Equality comparison for scan-path criteria: coerces the criterion to the property's
    /// declared type first (aligning scan semantics with the indexed path's typed comparison),
    /// falling back to plain equality when coercion is not possible.
    /// </summary>
    private static bool CriterionMatches(object? propertyValue, object? criterionValue, Type propertyType)
    {
        if (criterionValue == null || propertyValue == null)
        {
            return Equals(propertyValue, criterionValue);
        }

        if (TryCoerceToPropertyType(criterionValue, propertyType, out object coercedValue))
        {
            return Equals(propertyValue, coercedValue);
        }

        return Equals(propertyValue, criterionValue);
    }

    /// <summary>
    /// Extracts the (column, value) equality pairs from a query filter's parameter tokens,
    /// mirroring <see cref="MatchesQueryFilter"/>: parameter tokens with a null column name and
    /// non-parameter tokens are ignored.  NOTE: like <see cref="MatchesQueryFilter"/>, this
    /// treats every parameter token as an equality comparison regardless of its operator
    /// (bam.data.objects#6) — the two methods must change together when operators are honored.
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
            if (!CriterionMatches(value, param.Value, prop.PropertyType))
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
                if (!CriterionMatches(itemValue, filterValue, prop.PropertyType))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
