using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

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
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()          .For<IObjectHashCalculator>().Use<ObjectHashCalculator>();
    }
    
    [UnitTest]
    public void GetObjectKey()
    {
        ObjectIdentifierFactory factory = Get<ObjectIdentifierFactory>();
        ObjectHashCalculator objectHasher = Get<ObjectHashCalculator>();
        ObjectData data = new ObjectData(new TestData());
        
        ulong keyHash = objectHasher.CalculateKeyHash(data);
        IObjectKey key = factory.GetObjectKeyFor(data);
        key.Key.ShouldEqual(keyHash);
    }
}