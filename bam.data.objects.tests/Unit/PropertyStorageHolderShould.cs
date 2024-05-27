using System.Text;
using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Application.Unit;

[UnitTestMenu("PropertyStorageHolderShould")]
public class PropertyStorageHolderShould: UnitTestMenuContainer
{
    public PropertyStorageHolderShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void GetSlot()
    {
        string testDataPath = Path.Combine(Environment.CurrentDirectory, nameof(GetSlot), "testData");
        ServiceRegistry serviceRegistry = Configure(ConfigureDependencies(testDataPath))
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();

        FsObjectStorageManager fsObjectStorageManager = serviceRegistry.Get<FsObjectStorageManager>();
        ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
        string propertyName = "StringProperty";
        
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 64.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };
        IObjectData objectData = dataFactory.Wrap(new ObjectData(testData));
        PropertyStorageHolder propertyStorageHolder =
            new PropertyStorageHolder(testDataPath);

        IPropertyStorageSlot propertyStorageSlot = propertyStorageHolder.GetPropertyVersionSlot(objectData.Property(propertyName), 1);
        
        propertyStorageSlot.ShouldNotBeNull("PropertyStorageSlot was null");
    }

    [UnitTest]
    public void GetVersionHolders()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetVersionHolders));
        ServiceRegistry serviceRegistry = Configure(ConfigureDependencies(root))
            .For<IObjectStorageManager>().Use<FsObjectStorageManager>();

        FsObjectStorageManager fsObjectStorageManager = serviceRegistry.Get<FsObjectStorageManager>();
        ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
        string propertyName = "StringProperty";
        
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 64.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };
        IObjectData objectData = dataFactory.Wrap(new ObjectData(testData));

        IPropertyHolder propertyHolder = fsObjectStorageManager.GetPropertyStorageHolder(objectData.Property(propertyName));
    }
    
    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath));

        ServiceRegistry serviceRegistry = Configure(testRegistry);
        return serviceRegistry;
    }
}