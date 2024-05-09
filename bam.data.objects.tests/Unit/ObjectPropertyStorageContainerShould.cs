using System.Text;
using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectPropertyStorageContainerShould")]
public class ObjectPropertyStorageContainerShould: UnitTestMenuContainer
{
    public ObjectPropertyStorageContainerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void HaveVersionPath()
    {
        string testDataPath = Path.Combine(Environment.CurrentDirectory, nameof(HaveVersionPath), "testData");
        string expectedPath = Path.Combine(testDataPath, "1");
        ObjectPropertyStorageContainer objectPropertyStorageContainer =
            new ObjectPropertyStorageContainer(testDataPath);
        objectPropertyStorageContainer.FullName.ShouldEqual(expectedPath);
    }
    
    [UnitTest]
    public void HaveVersion()
    {
        ObjectPropertyStorageContainer objectPropertyStorageContainer =
            new ObjectPropertyStorageContainer(Path.Combine(Environment.CurrentDirectory, nameof(HaveVersion), "testData"));
        objectPropertyStorageContainer.Version.ShouldNotBeNull();
        objectPropertyStorageContainer.Version.Number.ShouldEqual(1);
    }

    [UnitTest]
    public void SaveObjectProperty()
    {
        ServiceRegistry testRegistry =
            ConfigureDependencies(Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty)));
        FsObjectStorageManager storageManager = testRegistry.Get<FsObjectStorageManager>();
        IObjectData objectData = new ObjectData(new TestData
        {
            StringProperty = $"StringProperty-SaveObjectPropertyTest"
        });
        IObjectProperty property = objectData.Property("StringProperty");
        IObjectPropertyStorageContainer propertyStorageContainer =
            storageManager.GetPropertyStorageContainer(property);
        
        IObjectPropertyWriteResult writeResult = propertyStorageContainer.Save(storageManager, property);
        Message.PrintLine(writeResult.StorageSlot.FullName);
        File.Exists(writeResult.StorageSlot.FullName).ShouldBeTrue($"{writeResult.StorageSlot.FullName} doesn't exist");
    }
    
    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IRootStorageContainer>().Use(new RootStorageContainer(rootPath));

        ServiceRegistry serviceRegistry = Configure(testRegistry);
        return serviceRegistry;
    }
}