using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Net;
using Bam.Net.CoreServices;
using Bam.Testing;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("ObjectPropertyStorageContainerShould")]
public class ObjectPropertyStorageContainerShould: UnitTestMenuContainer
{
    public ObjectPropertyStorageContainerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WriteDatFileToExpectedPath()
    {
        string rootPath = Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty));
        ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();
        
        ObjectDataFactory dataFactory = testRegistry.Get<ObjectDataFactory>();
        IObjectStorageManager storageManager = testRegistry.Get<IObjectStorageManager>();
        
        ObjectPropertyStorageHolder storageHolder = testRegistry.Get<ObjectPropertyStorageHolder>(new string[] {rootPath});
        
        IObjectData objectData = dataFactory.Wrap(new TestData
        {
            StringProperty = $"StringProperty-SaveObjectPropertyTest"
        });

        IObjectKey key = objectData.GetObjectKey();
        
        List<string> pathSegments = new List<string> { rootPath, "objects" };
        pathSegments.AddRange(typeof(TestData).Namespace.Split('.'));
        pathSegments.AddRange(key.Key.Split(2));
        pathSegments.Add("StringProperty");
        pathSegments.Add("1");
        pathSegments.Add("dat");
        
        string expected = Path.Combine(pathSegments.ToArray());
        IObjectPropertyWriteResult writeResult = storageHolder.Save(storageManager, objectData.Property("StringProperty"));
        writeResult.StorageSlot.FullName.ShouldEqual(expected);
    }

    [UnitTest]
    public void SaveObjectProperty()
    {
        ServiceRegistry testRegistry =
            IntegrationTests.ConfigureDependencies(Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty)));
        FsObjectStorageManager storageManager = testRegistry.Get<FsObjectStorageManager>();
        IObjectData objectData = new ObjectData(new TestData
        {
            StringProperty = $"StringProperty-SaveObjectPropertyTest"
        });
        IObjectProperty property = objectData.Property("StringProperty");
        IObjectPropertyStorageHolder propertyStorageHolder =
            storageManager.GetPropertyStorageContainer(property);
        
        IObjectPropertyWriteResult writeResult = propertyStorageHolder.Save(storageManager, property);
        Message.PrintLine(writeResult.StorageSlot.FullName);
        File.Exists(writeResult.StorageSlot.FullName).ShouldBeTrue($"{writeResult.StorageSlot.FullName} doesn't exist");
    }
}