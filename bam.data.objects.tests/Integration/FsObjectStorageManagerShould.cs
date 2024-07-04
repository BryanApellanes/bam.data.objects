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
public class FsObjectStorageManagerShould : UnitTestMenuContainer
{
    public FsObjectStorageManagerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }


    [UnitTest]
    public async Task WriteAndReadProperty()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteAndReadProperty));
        ServiceRegistry serviceRegistry = IntegrationTests.ConfigureDependencies(root);
        serviceRegistry
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();

        FsObjectStorageManager fsObjectStorageManager = serviceRegistry.Get<FsObjectStorageManager>();

        IObjectDataFactory objectDataFactory = serviceRegistry.Get<IObjectDataFactory>();
        IObjectData testObjectData = objectDataFactory.Wrap(new TestData(true));
        
        IProperty stringProperty = testObjectData.Property(nameof(TestData.StringProperty));
        string stringPropertyValue = stringProperty.Value;
        IPropertyWriteResult propertyWriteResult = fsObjectStorageManager.WriteProperty(stringProperty);
        Message.PrintLine(propertyWriteResult.PointerStorageSlot.FullName);
        //IProperty propertyRead = fsObjectStorageManager.ReadProperty(stringProperty);
        throw new NotImplementedException();
    }
}