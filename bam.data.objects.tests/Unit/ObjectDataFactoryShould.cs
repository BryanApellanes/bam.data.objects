using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Data.Objects.Tests.Integration;
using Bam.Net.CoreServices;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectDataFactoryShould")]
public class ObjectDataFactoryShould : UnitTestMenuContainer
{
    public ObjectDataFactoryShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task SetObjectIdentifierFactoryOnObjectData()
    {
        string rootPath = Path.Combine(Environment.CurrentDirectory, nameof(SetObjectIdentifierFactoryOnObjectData));
        ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();
        
        ObjectDataFactory dataFactory = testRegistry.Get<ObjectDataFactory>();
        
        IObjectData objectData = dataFactory.Wrap(new TestData(true));
        
        objectData.ObjectIdentifierFactory.ShouldNotBeNull($"{nameof(objectData.ObjectIdentifierFactory)} was null");
        IObjectKey objectKey = objectData.GetObjectKey();
        objectKey.ShouldNotBeNull("objectKey was null");
        Message.PrintLine(objectKey.ToString());
    }
}