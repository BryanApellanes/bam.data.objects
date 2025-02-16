using Bam.Data.Repositories;

namespace Bam.Data.Objects;

public class ObjectDataRepository : Repository
{
    public ObjectDataRepository(IObjectDataFactory factory, IObjectDataWriter writer, IObjectDataIndexer indexer, IObjectDataDeleter deleter, IObjectDataArchiver archiver, IObjectDataReader reader, IObjectDataSearcher searcher)
    {
        this.Factory = factory;
        this.Writer = writer;
        this.Indexer = indexer;
        this.Deleter = deleter;
        this.Archiver = archiver;
        this.Reader = reader;
        this.Searcher = searcher;
    }
    
    protected IObjectDataFactory Factory { get; }
    protected IObjectDataWriter Writer { get; }
    protected IObjectDataIndexer Indexer { get; }
    protected IObjectDataDeleter Deleter { get; }
    protected IObjectDataArchiver Archiver { get; }
    protected IObjectDataReader Reader { get; }
    protected IObjectDataSearcher Searcher { get; }
    
    public override T Create<T>(T toCreate)
    {
        this.Writer.WriteAsync(Factory.GetObjectData(toCreate));
        return toCreate;
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

    public override T Retrieve<T>(ulong id)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<T> Query<T>(IQueryFilter query)
    {
        throw new NotImplementedException();
    }

    public override T Retrieve<T>(string uuid)
    {
        throw new NotImplementedException();
    }

    public override object Retrieve(Type objectType, long id)
    {
        throw new NotImplementedException();
    }

    public override object Retrieve(Type objectType, ulong id)
    {
        throw new NotImplementedException();
    }

    public override object Retrieve(Type objectType, string uuid)
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

    public override T Retrieve<T>(int id)
    {
        throw new NotImplementedException();
    }

    public override T Retrieve<T>(long id)
    {
        throw new NotImplementedException();
    }
}