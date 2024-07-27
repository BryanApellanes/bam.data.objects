using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Test;
using NSubstitute;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectIdentifierFactory should")]
public class ObjectIdentifierFactoryShould : UnitTestMenuContainer
{
    public ObjectIdentifierFactoryShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        IObjectDataStorageManager mockObjectDataStorageManager = Substitute.For<IObjectDataStorageManager>();

        return base.Configure(serviceRegistry)
            .For<IObjectDataStorageManager>().Use(mockObjectDataStorageManager)
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()          .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>();
    }
    
    [UnitTest]
    public void GetObjectKey()
    {
        ObjectDataIdentifierFactory factory = Get<ObjectDataIdentifierFactory>();
        ObjectDataIdentityCalculator objectDataIdentityHasher = Get<ObjectDataIdentityCalculator>();
        ObjectData data = new ObjectData(new TestData());
        
        ulong keyHash = objectDataIdentityHasher.CalculateULongKey(data);
        IObjectDataKey dataKey = factory.GetObjectKey(data);
        dataKey.Key.ShouldEqual(keyHash);
    }
}