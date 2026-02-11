using System.Reflection;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

public class ObjectDataRepository : AsyncRepository
{
    public ObjectDataRepository(IObjectDataFactory factory, IObjectDataWriter writer, IObjectDataIndexer indexer, IObjectDataDeleter deleter, IObjectDataArchiver archiver, IObjectDataReader reader, IObjectDataSearcher searcher, ICompositeKeyCalculator compositeKeyCalculator)
    {
        this.Factory = factory;
        this.Writer = writer;
        this.Indexer = indexer;
        this.Deleter = deleter;
        this.Archiver = archiver;
        this.Reader = reader;
        this.Searcher = searcher;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    protected IObjectDataFactory Factory { get; }
    protected IObjectDataWriter Writer { get; }
    protected IObjectDataIndexer Indexer { get; }
    protected IObjectDataDeleter Deleter { get; }
    protected IObjectDataArchiver Archiver { get; }
    protected IObjectDataReader Reader { get; }
    protected IObjectDataSearcher Searcher { get; }
    protected ICompositeKeyCalculator CompositeKeyCalculator { get; }

    public override T Create<T>(T toCreate)
    {
        IObjectData objectData = Factory.GetObjectData(toCreate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(typeof(T));
        keyProp?.SetValue(toCreate, id);

        Task<IObjectDataWriteResult> writeTask = Writer.WriteAsync(objectData);
        writeTask.GetAwaiter().GetResult();

        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toCreate;
    }

    public override object Create(object toCreate)
    {
        IObjectData objectData = Factory.GetObjectData(toCreate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(toCreate.GetType());
        keyProp?.SetValue(toCreate, id);

        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toCreate;
    }

    public override object Create(Type type, object toCreate)
    {
        return Create(toCreate);
    }

    public override T Retrieve<T>(ulong id)
    {
        IObjectDataKey key = Indexer.LookupAsync(typeof(T), id).GetAwaiter().GetResult();
        if (key == null)
        {
            return default;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        if (result?.ObjectData?.Data == null)
        {
            return default;
        }

        return (T)result.ObjectData.Data;
    }

    public override T Retrieve<T>(int id)
    {
        return Retrieve<T>((ulong)id);
    }

    public override T Retrieve<T>(long id)
    {
        return Retrieve<T>((ulong)id);
    }

    public override T Retrieve<T>(string uuid)
    {
        IObjectDataKey key = Indexer.LookupByUuidAsync(typeof(T), uuid).GetAwaiter().GetResult();
        if (key == null)
        {
            return default;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        if (result?.ObjectData?.Data == null)
        {
            return default;
        }

        return (T)result.ObjectData.Data;
    }

    public override object Retrieve(Type objectType, long id)
    {
        return Retrieve(objectType, (ulong)id);
    }

    public override object Retrieve(Type objectType, ulong id)
    {
        IObjectDataKey key = Indexer.LookupAsync(objectType, id).GetAwaiter().GetResult();
        if (key == null)
        {
            return null;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        return result?.ObjectData?.Data;
    }

    public override object Retrieve(Type objectType, string uuid)
    {
        IObjectDataKey key = Indexer.LookupByUuidAsync(objectType, uuid).GetAwaiter().GetResult();
        if (key == null)
        {
            return null;
        }

        IObjectDataReadResult result = Reader.ReadObjectDataAsync(key).GetAwaiter().GetResult();
        return result?.ObjectData?.Data;
    }

    public override T Update<T>(T toUpdate)
    {
        IObjectData objectData = Factory.GetObjectData(toUpdate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(typeof(T));
        keyProp?.SetValue(toUpdate, id);

        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toUpdate;
    }

    public override object Update(object toUpdate)
    {
        IObjectData objectData = Factory.GetObjectData(toUpdate);

        ulong id = CompositeKeyCalculator.CalculateULongKey(objectData);
        PropertyInfo keyProp = GetKeyProperty(toUpdate.GetType());
        keyProp?.SetValue(toUpdate, id);

        Writer.WriteAsync(objectData).GetAwaiter().GetResult();
        Indexer.IndexAsync(objectData).GetAwaiter().GetResult();

        return toUpdate;
    }

    public override object Update(Type type, object toUpdate)
    {
        return Update(toUpdate);
    }

    public override bool Delete<T>(T toDelete)
    {
        IObjectData objectData = Factory.GetObjectData(toDelete);
        IObjectDataDeleteResult result = Deleter.DeleteAsync(objectData).GetAwaiter().GetResult();
        return result.Success;
    }

    public override bool Delete(object toDelete)
    {
        IObjectData objectData = Factory.GetObjectData(toDelete);
        IObjectDataDeleteResult result = Deleter.DeleteAsync(objectData).GetAwaiter().GetResult();
        return result.Success;
    }

    public override bool Delete(Type type, object toDelete)
    {
        return Delete(toDelete);
    }

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

    public override IEnumerable<T> Query<T>(Func<T, bool> query)
    {
        return RetrieveAll<T>().Where(query);
    }

    public override IEnumerable<object> Query(Type type, Func<object, bool> predicate)
    {
        return RetrieveAll(type).Where(predicate);
    }

    public override IEnumerable<T> Query<T>(Dictionary<string, object> queryParameters)
    {
        return RetrieveAll<T>().Where(item => MatchesQueryParameters(item, queryParameters));
    }

    public override IEnumerable<object> Query(Type type, Dictionary<string, object> queryParameters)
    {
        return RetrieveAll(type).Where(item => MatchesQueryParameters(item, queryParameters));
    }

    public override IEnumerable<T> Query<T>(IQueryFilter query)
    {
        return RetrieveAll<T>().Where(item => MatchesQueryFilter(item, query));
    }

    public override IEnumerable<object> Query(Type type, IQueryFilter query)
    {
        return RetrieveAll(type).Where(item => MatchesQueryFilter(item, query));
    }

    public override IEnumerable<object> Query(string propertyName, object propertyValue)
    {
        if (DefaultType == null)
        {
            return Enumerable.Empty<object>();
        }

        return RetrieveAll(DefaultType).Where(item =>
        {
            PropertyInfo prop = item.GetType().GetProperty(propertyName);
            if (prop == null)
            {
                return false;
            }

            object value = prop.GetValue(item);
            return Equals(value, propertyValue);
        });
    }

    private static bool MatchesQueryParameters(object item, Dictionary<string, object> queryParameters)
    {
        foreach (KeyValuePair<string, object> param in queryParameters)
        {
            PropertyInfo prop = item.GetType().GetProperty(param.Key);
            if (prop == null)
            {
                return false;
            }

            object value = prop.GetValue(item);
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
                string columnName = parameterInfo.ColumnName;
                object filterValue = parameterInfo.Value;
                if (columnName == null)
                {
                    continue;
                }

                PropertyInfo prop = item.GetType().GetProperty(columnName);
                if (prop == null)
                {
                    return false;
                }

                object itemValue = prop.GetValue(item);
                if (!Equals(itemValue, filterValue))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
