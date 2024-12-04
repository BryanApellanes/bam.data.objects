using System.Text;
using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Storage;
using Bam.Test;

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
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(testDataPath))
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();
        ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
        string propertyName = "StringProperty";
        
        PlainTestClass plainTestClass = new PlainTestClass
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 64.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };
        IObjectData objectData = dataFactory.GetObjectData(new ObjectData(plainTestClass));
        PropertyStorageHolder propertyStorageHolder =
            new PropertyStorageHolder(testDataPath);

        IPropertyStorageSlot propertyStorageSlot = propertyStorageHolder.GetPropertyVersionSlot(fsObjectDataStorageManager, objectData.Property(propertyName), 1);
        
        propertyStorageSlot.ShouldNotBeNull("PropertyStorageSlot was null");
    }

    [UnitTest]
    public void GetVersionHolders()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetVersionHolders));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();
        ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
        string propertyName = "StringProperty";
        
        PlainTestClass plainTestClass = new PlainTestClass
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 64.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };
        IObjectData objectData = dataFactory.GetObjectData(new ObjectData(plainTestClass));

        IPropertyStorageHolder propertyStorageHolder = fsObjectDataStorageManager.GetPropertyStorageHolder(objectData.Property(propertyName).ToDescriptor());
        
        propertyStorageHolder.ShouldNotBeNull($"{nameof(propertyStorageHolder)} was null");
    }
    
    public ServiceRegistry ConfigureTestRegistry(ServiceRegistry serviceRegistry)
    {
        return Configure(serviceRegistry)
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath));

        ServiceRegistry serviceRegistry = ConfigureTestRegistry(testRegistry);
        return serviceRegistry;
    }
}