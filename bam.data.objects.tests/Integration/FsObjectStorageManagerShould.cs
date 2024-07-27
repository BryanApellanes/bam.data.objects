using Bam.Console;
using Bam.CoreServices;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.Shell;
using Bam.Test;
using Bam.Test.Integration;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("FsObjectStorageManager Should")]
public class FsObjectStorageManagerShould(ServiceRegistry serviceRegistry) : UnitTestMenuContainer(serviceRegistry)
{
    [UnitTest]
    public Task WriteAndReadProperty()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteAndReadProperty));
        ServiceRegistry serviceRegistry = IntegrationTests.ConfigureDependencies(root);
        serviceRegistry
            .For<IObjectDecoder>().Use<JsonObjectDataEncoder>()
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();

        IObjectDataFactory objectDataFactory = serviceRegistry.Get<IObjectDataFactory>();
        IObjectData testObjectData = objectDataFactory.Wrap(new TestData(true));
        
        IProperty? stringProperty = testObjectData.Property(nameof(TestData.StringProperty));
        stringProperty?.ShouldNotBeNull();
        string? stringPropertyValue = stringProperty?.Value;
        
        IPropertyWriteResult propertyWriteResult = fsObjectDataStorageManager.WriteProperty(stringProperty!);
        
        Message.PrintLine(propertyWriteResult.PointerStorageSlot.FullName);
        
        IProperty readProperty = fsObjectDataStorageManager.ReadProperty(new ObjectData(propertyWriteResult.ObjectDataKey.TypeDescriptor), propertyWriteResult.GetDescriptor());
        readProperty.Value.ShouldBeEqualTo(stringPropertyValue!);
        return Task.CompletedTask;
    }
}