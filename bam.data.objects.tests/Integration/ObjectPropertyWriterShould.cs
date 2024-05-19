using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Net;
using Bam.Net.CoreServices;
using Bam.Net.Data.Repositories;
using Bam.Testing;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("ObjectPropertyWriterShould")]
public class ObjectPropertyWriterShould: UnitTestMenuContainer
{
    public ObjectPropertyWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WritePropertyDatFile()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WritePropertyDatFile));
        ServiceRegistry testContainer = IntegrationTests.ConfigureDependencies(root);
        testContainer
            .For<IObjectPropertyWriter>().Use<ObjectPropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();

        IObjectDataFactory dataFactory = testContainer.Get<ObjectDataFactory>();
        IObjectPropertyWriter propertyWriter = testContainer.Get<ObjectPropertyWriter>();
        
        IObjectData testData = dataFactory.Wrap(new TestData(true));
        IObjectProperty property = testData.Property("StringProperty");
        property.ShouldNotBeNull("String property was null");
        IObjectPropertyWriteResult result = await propertyWriter.WritePropertyAsync(property);
        
        
    } 
}