using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Data.Repositories;
using Bam.Test;
using NSubstitute;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectIdentifierFactory should")]
public class ObjectIdentifierFactoryShould : UnitTestMenuContainer
{
    public ObjectIdentifierFactoryShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }
    
    [UnitTest]
    public void GetObjectKey()
    {
        ObjectDataLocatorFactory factory = Get<ObjectDataLocatorFactory>();
        ObjectDataIdentityCalculator objectDataIdentityHasher = Get<ObjectDataIdentityCalculator>();
        ObjectData data = new ObjectData(new PlainTestClass());
        
        ulong keyHash = objectDataIdentityHasher.CalculateULongKey(data);
        IObjectDataKey dataKey = factory.GetObjectKey(data);
        dataKey.Key.ShouldEqual(keyHash);
    }

    [UnitTest]
    public void CalculateSameKeyForEqualCompositeKeyValues()
    {
        string uuid = Guid.NewGuid().ToString();
        string cuid = Cuid.Generate();
        TestRepoData data1 = new TestRepoData()
        {
            Uuid = uuid,
            Cuid = cuid
        };
        TestRepoData data2 = new TestRepoData()
        {
            Uuid = uuid,
            Cuid = cuid
        };
        ObjectDataFactory dataFactory = Get<ObjectDataFactory>();
        ObjectDataLocatorFactory factory = Get<ObjectDataLocatorFactory>();

        IObjectData wrapped1 = dataFactory.Wrap(data1);
        IObjectData wrapped2 = dataFactory.Wrap(data2);
        IObjectDataKey key1 = factory.GetObjectKey(wrapped1);
        IObjectDataKey key2 = factory.GetObjectKey(wrapped2);

        key1.Equals(key2).ShouldBeTrue();
    }

    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        IObjectDataStorageManager mockObjectDataStorageManager = Substitute.For<IObjectDataStorageManager>();

        return base.Configure(serviceRegistry)
            .For<IObjectDataStorageManager>().Use(mockObjectDataStorageManager)
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>();
    }
}