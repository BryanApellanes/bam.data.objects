using Bam.CoreServices;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.Shell;
using Bam.Testing;
using Bam.Testing.Integration;

namespace Bam.Data.Objects.Tests.Integration;

[Menu("FsObjectStorageManager Should")]
public class FsObjectStorageManagerShould : UnitTestMenuContainer
{
    public FsObjectStorageManagerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }


    [IntegrationTest]
    public async Task WriteAndReadProperty()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteAndReadProperty));
        ServiceRegistry serviceRegistry = IntegrationTests.ConfigureDependencies(root);
        serviceRegistry
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();

        FsObjectStorageManager fsObjectStorageManager = serviceRegistry.Get<FsObjectStorageManager>();

        ObjectData<TestData> testObjectData = new ObjectData<TestData>(new TestData(true));
        
        IProperty stringProperty = testObjectData.Property(nameof(TestData.StringProperty));
        string stringPropertyValue = stringProperty.Value;
        fsObjectStorageManager.WriteProperty(stringProperty);
        //IProperty propertyRead = fsObjectStorageManager.ReadProperty(stringProperty);
        throw new NotImplementedException();
    }
}