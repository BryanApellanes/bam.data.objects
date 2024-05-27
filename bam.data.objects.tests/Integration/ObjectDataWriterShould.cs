using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam;
using Bam.CoreServices;
using Bam.Data.Repositories;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Data.Objects.Tests.Integration;

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
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectCalculator>().Use<ObjectCalculator>()
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
            IPropertyWriteResult propertyWriteResult = result.PropertyWriteResults[key];
            
            propertyWriteResult.ShouldNotBeNull("propertyWriteResult was null");
            propertyWriteResult.Property.ShouldNotBeNull("propertyWriteResult.ObjectProperty was null");
            propertyWriteResult.RawData.ShouldNotBeNull("propertyWriteResult.RawData was null");
        }
        
    }

    public async Task WritePropertyFiles()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WritePropertyFiles));
        ServiceRegistry testContainer = ConfigureDependencies(root);
        testContainer
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>();
        
        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();
        IObjectStorageManager storageManager = testContainer.Get<IObjectStorageManager>();
        TestData testData = new TestData(true);

        IObjectDataWriteResult result = await objectDataWriter.WriteAsync(testData);
        // validate against newly documented implementation
        result.PropertyWriteResults.Count.ShouldEqual(4);
        foreach (string key in result.PropertyWriteResults.Keys)
        {
            
            IPropertyWriteResult propertyWriteResult = result.PropertyWriteResults[key];
            //propertyWriteResult.StorageSlot.FullName.ShouldEqual();
        }
        // rewrite implementation to conform to the README

        throw new NotImplementedException();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath);

        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}