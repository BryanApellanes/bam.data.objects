using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Net.Data.Repositories;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Integration;

[UnitTestMenu("Integration: ObjectDataWriter should")]
public class ObjectDataWriterShould: UnitTestMenuContainer
{
    public ObjectDataWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }
    
    [UnitTest]
    public async Task WriteData()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteData));
        ServiceRegistry testContainer = ConfigureDependencies(root);
        testContainer
            .For<IObjectPropertyWriter>().Use<ObjectPropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>()
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>();
        
        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();
        
        TestData testData = new TestData(true);

        IObjectDataWriteResult result = await objectDataWriter.WriteAsync(testData);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue(result.Message);
        result.ObjectData.ShouldNotBeNull("result.Data was null");
        result.ObjectData.Data.ShouldBe(testData);
        result.ObjectKey.ShouldNotBeNull("result.ObjectKey was null");
        result.KeySlot.ShouldNotBeNull("result.KeySlot was null");
        
        result.PropertyWriteResults.Count.ShouldEqual(4, "result.PropertyWriteResults.Count was not equal to 4");
        
        foreach (string key in result.PropertyWriteResults.Keys)
        {
            IObjectPropertyWriteResult propertyWriteResult = result.PropertyWriteResults[key];
            
            propertyWriteResult.ShouldNotBeNull("propertyWriteResult was null");
            propertyWriteResult.ObjectProperty.ShouldNotBeNull("propertyWriteResult.ObjectProperty was null");
            propertyWriteResult.RawData.ShouldNotBeNull("propertyWriteResult.RawData was null");
        }
        
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>()
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IRootStorageContainer>().Use( new RootStorageContainer(rootPath))
            .For<IStorageIdentifier>().Use(new FsStorageContainer(rootPath));

        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}