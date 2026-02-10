using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;
using NSubstitute;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectIdentifierFactory should")]
public class ObjectIdentifierFactoryShould : UnitTestMenuContainer
{
    public ObjectIdentifierFactoryShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
        Configure((svcRegistry) =>
        {
            IObjectDataStorageManager mockObjectDataStorageManager = Substitute.For<IObjectDataStorageManager>();

            svcRegistry
                .For<IObjectDataStorageManager>().Use(mockObjectDataStorageManager)
                .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
                .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
                .For<IHashCalculator>().Use<JsonHashCalculator>()
                .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
                .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>();
        });
    }

    [UnitTest]
    public void GetObjectKey()
    {
        ObjectDataLocatorFactory factory = Get<ObjectDataLocatorFactory>();
        ObjectDataIdentityCalculator objectDataIdentityHasher = Get<ObjectDataIdentityCalculator>();

        When.A<PlainTestClass>("is wrapped in ObjectData and its key is calculated", (ptc) =>
        {
            ObjectData data = new ObjectData(ptc);
            string keyHash = objectDataIdentityHasher.CalculateHashHexKey(data);
            IObjectDataKey dataKey = factory.GetObjectKey(data);
            return new object[] { keyHash, dataKey.Key };
        })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            string keyHash = (string)results[0];
            string dataKeyKey = (string)results[1];
            because.ItsTrue("dataKey equals calculated hash", dataKeyKey.Equals(keyHash));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CalculateSameKeyForEqualCompositeKeyValues()
    {
        string uuid = Guid.NewGuid().ToString();
        string cuid = Cuid.Generate();
        ObjectDataFactory dataFactory = Get<ObjectDataFactory>();
        ObjectDataLocatorFactory factory = Get<ObjectDataLocatorFactory>();

        When.A<TestRepoData>("with same composite key values produces the same key",
            () => new TestRepoData { Uuid = uuid, Cuid = cuid },
            (data1) =>
            {
                TestRepoData data2 = new TestRepoData { Uuid = uuid, Cuid = cuid };
                IObjectData wrapped1 = dataFactory.GetObjectData(data1);
                IObjectData wrapped2 = dataFactory.GetObjectData(data2);
                IObjectDataKey key1 = factory.GetObjectKey(wrapped1);
                IObjectDataKey key2 = factory.GetObjectKey(wrapped2);
                return key1.Equals(key2);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("keys are equal", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
