using Bam.Data;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

public class ObjectRepository : Repository
{
    public ObjectRepository(IObjectDataWriter writer, IObjectDataIndexer dataIndexer, IObjectDataDeleter dataDeleter, IObjectDataArchiver dataArchiver, IObjectDataReader dataReader, IObjectDataSearcher dataSearcher)
    {
        this.Writer = writer;
        this.DataIndexer = dataIndexer;
        this.DataDeleter = dataDeleter;
        this.DataArchiver = dataArchiver;
        this.DataReader = dataReader;
        this.DataSearcher = dataSearcher;
    }
    
    protected IObjectDataWriter Writer { get; }
    protected IObjectDataIndexer DataIndexer { get; }
    protected IObjectDataDeleter DataDeleter { get; }
    protected IObjectDataArchiver DataArchiver { get; }
    protected IObjectDataReader DataReader { get; }
    protected IObjectDataSearcher DataSearcher { get; }
    
    public override T Create<T>(T toCreate)
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