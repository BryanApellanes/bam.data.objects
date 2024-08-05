using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam;
using Bam.CoreServices;
using Bam.Data.Repositories;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("PropertyWriterShould")]
public class PropertyWriterShould: UnitTestMenuContainer
{
    public PropertyWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WritePropertyDatFile()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WritePropertyDatFile));
        string propertyName = "StringProperty"; // defined in TestData class
        
        ServiceRegistry testContainer = IntegrationTests.ConfigureDependencies(root);
        testContainer
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
        
        IObjectDataFactory dataFactory = testContainer.Get<ObjectDataFactory>();
        IObjectData testData = dataFactory.Wrap(new PlainTestClass(true));
        IObjectDataKey objectDataKey = testData.GetObjectKey();
        IObjectDataStorageManager objectDataStorageManager = testContainer.Get<FsObjectDataStorageManager>();
        
        objectDataKey.GetPath(objectDataStorageManager).StartsWith(root).ShouldBeTrue("objectKey path was not in correct root");
        
        List<string> expectedParts = new List<string>();
        expectedParts.Add(objectDataKey.GetPath(objectDataStorageManager));
        expectedParts.Add(propertyName);
        expectedParts.Add("1");
        expectedParts.Add("dat");
        
        IPropertyWriter propertyWriter = testContainer.Get<PropertyWriter>();
        IProperty property = testData.Property(propertyName);
        property.ShouldNotBeNull("String property was null");
        IPropertyWriteResult result = await propertyWriter.WritePropertyAsync(property);
        result.PointerStorageSlot.FullName.ShouldEqual(Path.Combine(expectedParts.ToArray()));
    } 
}