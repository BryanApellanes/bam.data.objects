using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

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
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>();
        
        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();
        
        PlainTestClass plainTestClass = new PlainTestClass(true);

        IObjectDataWriteResult result = await objectDataWriter.WriteAsync(plainTestClass);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue(result.Message);
        result.ObjectData.ShouldNotBeNull("result.Data was null");
        result.ObjectData.Data.ShouldBe(plainTestClass);
        result.ObjectDataKey.ShouldNotBeNull("result.ObjectKey was null");
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
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>();
        
        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();
        IObjectDataStorageManager dataStorageManager = testContainer.Get<IObjectDataStorageManager>();
        PlainTestClass plainTestClass = new PlainTestClass(true);

        IObjectDataWriteResult result = await objectDataWriter.WriteAsync(plainTestClass);
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