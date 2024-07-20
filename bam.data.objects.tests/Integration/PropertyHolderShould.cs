using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam;
using Bam.CoreServices;
using Bam.Testing;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("PropertyHolderShould")]
public class PropertyHolderShould: UnitTestMenuContainer
{
    public PropertyHolderShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WriteDatFileToExpectedPath()
    {
        string rootPath = Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty));
        ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
        //testRegistry.Diagnostic = true;
        ObjectDataFactory dataFactory = testRegistry.Get<ObjectDataFactory>();
        IObjectDataStorageManager dataStorageManager = testRegistry.Get<IObjectDataStorageManager>();
        
        PropertyStorageHolder storageHolder = testRegistry.Get<PropertyStorageHolder>(new string[] {rootPath});
        
        IObjectData objectData = dataFactory.Wrap(new TestData
        {
            StringProperty = $"StringProperty-SaveObjectPropertyTest"
        });

        IObjectDataKey dataKey = objectData.GetObjectKey();
        
        List<string> pathSegments = new List<string> { rootPath, "objects" };
        pathSegments.AddRange(typeof(TestData).Namespace.Split('.'));
        pathSegments.AddRange(dataKey.Key.Split(2));
        pathSegments.Add("StringProperty");
        pathSegments.Add("1");
        pathSegments.Add("dat");
        
        string expected = Path.Combine(pathSegments.ToArray());
        if (File.Exists(expected))
        {
            File.Delete(expected);
        }
        IPropertyWriteResult writeResult = storageHolder.Save(dataStorageManager, objectData.Property("StringProperty"));
        writeResult.PointerStorageSlot.FullName.ShouldEqual(expected);
        File.Exists(writeResult.PointerStorageSlot.FullName).ShouldBeTrue("file didn't exist");
    }

    [UnitTest]
    public void SaveObjectProperty()
    {
        ServiceRegistry testRegistry =
            IntegrationTests.ConfigureDependencies(Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty)));
        FsObjectDataStorageManager dataStorageManager = testRegistry.Get<FsObjectDataStorageManager>();
        IObjectData objectData = new ObjectData(new TestData
        {
            StringProperty = $"StringProperty-SaveObjectPropertyTest"
        });
        IProperty property = objectData.Property("StringProperty");
        IPropertyStorageHolder propertyStorageHolder =
            dataStorageManager.GetPropertyStorageHolder(property.ToDescriptor());
        
        IPropertyWriteResult writeResult = propertyStorageHolder.Save(dataStorageManager, property);
        Message.PrintLine(writeResult.PointerStorageSlot.FullName);
        File.Exists(writeResult.PointerStorageSlot.FullName).ShouldBeTrue($"{writeResult.PointerStorageSlot.FullName} doesn't exist");
    }
}