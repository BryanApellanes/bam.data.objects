using Bam.Console;
using Bam.CoreServices;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.Shell;
using Bam.Testing;
using Bam.Testing.Integration;

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
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();

        FsObjectStorageManager fsObjectStorageManager = serviceRegistry.Get<FsObjectStorageManager>();

        IObjectDataFactory objectDataFactory = serviceRegistry.Get<IObjectDataFactory>();
        IObjectData testObjectData = objectDataFactory.Wrap(new TestData(true));
        
        IProperty? stringProperty = testObjectData.Property(nameof(TestData.StringProperty));
        stringProperty?.ShouldNotBeNull();
        string? stringPropertyValue = stringProperty?.Value;
        
        IPropertyWriteResult propertyWriteResult = fsObjectStorageManager.WriteProperty(stringProperty!);
        
        Message.PrintLine(propertyWriteResult.PointerStorageSlot.FullName);
        
        IProperty readProperty = fsObjectStorageManager.ReadProperty(propertyWriteResult.GetDescriptor());
        readProperty.Value.ShouldBeEqualTo(stringPropertyValue!);
        return Task.CompletedTask;
    }
}