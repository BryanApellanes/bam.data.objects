using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Data.Objects.Tests.Integration;
using Bam.CoreServices;
using Bam.Test;

namespace Bam.Application.Unit;

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
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
        
        ObjectDataFactory dataFactory = testRegistry.Get<ObjectDataFactory>();
        
        IObjectData objectData = dataFactory.GetObjectData(new PlainTestClass(true));
        
        objectData.ObjectDataLocatorFactory.ShouldNotBeNull($"{nameof(objectData.ObjectDataLocatorFactory)} was null");
        IObjectDataKey objectDataKey = objectData.GetObjectKey();
        objectDataKey.ShouldNotBeNull("objectKey was null");
        Message.PrintLine(objectDataKey.ToString());
    }
}