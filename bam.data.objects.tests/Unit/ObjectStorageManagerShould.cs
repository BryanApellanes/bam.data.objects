using Bam.Console;
using Bam.Data.Objects;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using bam.data.objects;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Data.Repositories;
using Bam.Incubation;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectStorageManager Should", "utosms")]
public class ObjectStorageManagerShould : UnitTestMenuContainer
{
    public ObjectStorageManagerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }
    
    [UnitTest]
    public void GetRootStorage()
    {
        string expected = Path.Combine(Environment.CurrentDirectory, nameof(GetRootStorage));
        DependencyProvider dependencyProvider = ConfigureDependencies(expected);
        FsObjectStorageManager storageManager = dependencyProvider.Get<FsObjectStorageManager>();

        string actual = storageManager.GetRootStorageHolder().FullName;
        actual.ShouldEqual(expected, $"root was not expected value {expected} but {actual}");
    }
    
    [UnitTest]
    public void GetTypeDirectoryForDynamicType()
    {        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectStorageManager ofs = dependencyProvider.Get<FsObjectStorageManager>();
        dynamic ob = new
        {
        };
        
        DirectoryInfo directoryInfo = ofs.GetTypeStorageHolder(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetTypeStorage()
    {        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectStorageManager fsObjectStorageManager = dependencyProvider.Get<FsObjectStorageManager>();
        TestData ob = new TestData();
        
        ITypeStorageHolder typeStorageHolder = fsObjectStorageManager.GetTypeStorageHolder(ob.GetType());
        typeStorageHolder.ShouldNotBeNull();
        typeStorageHolder.RootStorageHolder.ShouldNotBeNull();
        
        Message.PrintLine("typeDir = '{0}'", typeStorageHolder.FullName);
    }
    
    [UnitTest]
    public void GetPropertyStorageContainer()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetPropertyStorageContainer));
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
        IObjectKey key = objectData.GetObjectKey();
        List<string> parts = new List<string>();
        parts.Add(key.ToString());
        parts.Add(propertyName);
        string expected = Path.Combine(parts.ToArray());

        IPropertyStorageHolder propertyStorage = fsObjectStorageManager.GetPropertyStorageHolder(objectData.Property(propertyName).ToDescriptor());
        propertyStorage.TypeStorageHolder.ShouldNotBeNull("TypeStorageHolder was null");
        
        propertyStorage.PropertyName.ShouldNotBeNull();
        propertyStorage.PropertyName.ShouldEqual(propertyName);
        propertyStorage.FullName.ShouldEqual(expected);
        Message.PrintLine(expected);
    }

    [UnitTest]
    public async Task GetNextPropertyStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetNextPropertyStorageVersionSlot));
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

        IPropertyStorageVersionSlot propertyStorageVersionSlot = fsObjectStorageManager.GetNextPropertyStorageVersionSlot(objectData.Property(propertyName));
        
        propertyStorageVersionSlot.ShouldNotBeNull($"{nameof(propertyStorageVersionSlot)} was null");
        Message.PrintLine(propertyStorageVersionSlot.FullName);
    }
    
    [UnitTest]
    public async Task SavePropertyInStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));
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
        IProperty property = objectData.Property(propertyName);

        IPropertyStorageVersionSlot propertyStorageVersionSlot =
            fsObjectStorageManager.GetNextPropertyStorageVersionSlot(property);

        propertyStorageVersionSlot.SetData(property.ToRawData());

        IPropertyWriteResult result = propertyStorageVersionSlot.Save(fsObjectStorageManager, property);
        
        result.PointerStorageSlot.FullName.ShouldBeEqualTo(propertyStorageVersionSlot.FullName);
        File.Exists(result.PointerStorageSlot.FullName).ShouldBeTrue("file slot was not written");
    }
    
        
    [UnitTest]
    public async Task GetStorageForPropertyStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));
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
        IProperty property = objectData.Property(propertyName);

        IPropertyStorageVersionSlot propertyStorageVersionSlot =
            fsObjectStorageManager.GetNextPropertyStorageVersionSlot(property);
        Message.PrintLine(propertyStorageVersionSlot.FullName);

        IStorage storage = fsObjectStorageManager.GetStorage(propertyStorageVersionSlot);
        IStorageSlot slot = storage.Save(property.ToRawData());
        slot.FullName.ShouldBeEqualTo(propertyStorageVersionSlot.FullName);
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
        return UnitTests.ConfigureDependencies(rootPath);
    }
}