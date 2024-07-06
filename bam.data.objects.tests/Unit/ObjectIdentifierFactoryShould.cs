using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Testing;
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
        IObjectStorageManager mockObjectStorageManager = Substitute.For<IObjectStorageManager>();

        return base.Configure(serviceRegistry)
            .For<IObjectStorageManager>().Use(mockObjectStorageManager)
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()          .For<IObjectIdentityCalculator>().Use<ObjectIdentityCalculator>();
    }
    
    [UnitTest]
    public void GetObjectKey()
    {
        ObjectIdentifierFactory factory = Get<ObjectIdentifierFactory>();
        ObjectIdentityCalculator objectIdentityHasher = Get<ObjectIdentityCalculator>();
        ObjectData data = new ObjectData(new TestData());
        
        ulong keyHash = objectIdentityHasher.CalculateULongKey(data);
        IObjectKey key = factory.GetObjectKey(data);
        key.Key.ShouldEqual(keyHash);
    }
}