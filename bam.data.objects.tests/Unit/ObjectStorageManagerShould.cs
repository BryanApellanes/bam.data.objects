using Bam.Console;
using Bam.Data.Objects;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Data.Repositories;
using Bam.Incubation;
using Bam.Storage;
using Bam.Test;
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
        FsObjectDataStorageManager dataStorageManager = dependencyProvider.Get<FsObjectDataStorageManager>();

        string actual = dataStorageManager.GetRootStorageHolder().FullName;
        actual.ShouldEqual(expected, $"root was not expected value {expected} but {actual}");
    }
    
    [UnitTest]
    public void GetTypeDirectoryForDynamicType()
    {        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectDataStorageManager ofs = dependencyProvider.Get<FsObjectDataStorageManager>();
        dynamic ob = new
        {
        };
        
        DirectoryInfo directoryInfo = ofs.GetObjectStorageHolder(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetTypeStorage()
    {        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectDataStorageManager fsObjectDataStorageManager = dependencyProvider.Get<FsObjectDataStorageManager>();
        TestData ob = new TestData();
        
        ITypeStorageHolder typeStorageHolder = fsObjectDataStorageManager.GetObjectStorageHolder(ob.GetType());
        typeStorageHolder.ShouldNotBeNull();
        typeStorageHolder.RootStorageHolder.ShouldNotBeNull();
        
        Message.PrintLine("typeDir = '{0}'", typeStorageHolder.FullName);
    }
    
    [UnitTest]
    public void GetPropertyStorageContainer()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetPropertyStorageContainer));
        ServiceRegistry serviceRegistry = Configure(ConfigureDependencies(root))
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();
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
        IObjectDataKey dataKey = objectData.GetObjectKey();
        List<string> parts = new List<string>();
        parts.Add(dataKey.ToString());
        parts.Add(propertyName);
        string expected = Path.Combine(parts.ToArray());

        IPropertyStorageHolder propertyStorage = fsObjectDataStorageManager.GetPropertyStorageHolder(objectData.Property(propertyName).ToDescriptor());
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
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();
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

        IPropertyStorageVersionSlot propertyStorageVersionSlot = fsObjectDataStorageManager.GetNextPropertyStorageVersionSlot(objectData.Property(propertyName));
        
        propertyStorageVersionSlot.ShouldNotBeNull($"{nameof(propertyStorageVersionSlot)} was null");
        Message.PrintLine(propertyStorageVersionSlot.FullName);
    }
    
    [UnitTest]
    public async Task SavePropertyInStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));
        ServiceRegistry serviceRegistry = Configure(ConfigureDependencies(root))
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();
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
            fsObjectDataStorageManager.GetNextPropertyStorageVersionSlot(property);

        propertyStorageVersionSlot.SetData(property.ToRawData());

        IPropertyWriteResult result = propertyStorageVersionSlot.Save(fsObjectDataStorageManager, property);
        
        result.PointerStorageSlot.FullName.ShouldBeEqualTo(propertyStorageVersionSlot.FullName);
        File.Exists(result.PointerStorageSlot.FullName).ShouldBeTrue("file slot was not written");
    }
    
        
    [UnitTest]
    public async Task GetStorageForPropertyStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));
        ServiceRegistry serviceRegistry = Configure(ConfigureDependencies(root))
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

        FsObjectDataStorageManager fsObjectDataStorageManager = serviceRegistry.Get<FsObjectDataStorageManager>();
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
            fsObjectDataStorageManager.GetNextPropertyStorageVersionSlot(property);
        Message.PrintLine(propertyStorageVersionSlot.FullName);

        IStorage storage = fsObjectDataStorageManager.GetStorage(propertyStorageVersionSlot);
        IStorageSlot slot = storage.Save(property.ToRawData());
        slot.FullName.ShouldBeEqualTo(propertyStorageVersionSlot.FullName);
        fsObjectDataStorageManager.IsSlotWritten(slot).ShouldBeTrue("slot was not written");
    }

    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        return UnitTests.ConfigureDependencies(rootPath);
    }
}