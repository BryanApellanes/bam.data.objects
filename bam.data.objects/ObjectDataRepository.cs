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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public override bool Delete<T>(T toDelete)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<object> Query(Type type, IQueryFilter query)
    {
        throw new NotImplementedException();
    }

    public override bool Delete(object toDelete)
    {
        throw new NotImplementedException();
    }

    public override bool Delete(Type type, object toDelete)
    {
        throw new NotImplementedException();
    }

    public override object Create(object toCreate)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<T> Query<T>(Func<T, bool> query)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<object> Query(Type type, Func<object, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public override T Update<T>(T toUpdate)
    {
        throw new NotImplementedException();
    }

    public override object Update(object toUpdate)
    {
        throw new NotImplementedException();
    }

    public override object Update(Type type, object toUpdate)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<T> Query<T>(Dictionary<string, object> queryParameters)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<object> Query(Type type, Dictionary<string, object> queryParameters)
    {
        throw new NotImplementedException();
    }

    public override object Create(Type type, object toCreate)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<T> Query<T>(IQueryFilter query)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<object> Query(string propertyName, object propertyValue)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<T> RetrieveAll<T>()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<object> RetrieveAll(Type type)
    {
        throw new NotImplementedException();
    }

    public override void BatchRetrieveAll(Type dtoOrPocoType, int batchSize, Action<IEnumerable<object>> processor)
    {
        throw new NotImplementedException();
    }
}
