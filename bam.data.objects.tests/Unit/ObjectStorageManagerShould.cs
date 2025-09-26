using Bam.Console;
using Bam.Data.Objects;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;
using Bam.Test;

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
        PlainTestClass ob = new PlainTestClass();
        
        ITypeStorageHolder typeStorageHolder = fsObjectDataStorageManager.GetObjectStorageHolder(ob.GetType());
        typeStorageHolder.ShouldNotBeNull();
        typeStorageHolder.RootStorageHolder.ShouldNotBeNull();
        
        Message.PrintLine("typeDir = '{0}'", typeStorageHolder.FullName);
    }
    
    [UnitTest]
    public void GetPropertyStorageContainer()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetPropertyStorageContainer));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
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
    public async Task GetNextPropertyStorageRevisionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetNextPropertyStorageRevisionSlot));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
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

        IPropertyStorageRevisionSlot propertyStorageRevisionSlot = fsObjectDataStorageManager.GetNextPropertyStorageRevisionSlot(objectData.Property(propertyName));
        
        propertyStorageRevisionSlot.ShouldNotBeNull($"{nameof(propertyStorageRevisionSlot)} was null");
        Message.PrintLine(propertyStorageRevisionSlot.FullName);
    }
    
    [UnitTest]
    public async Task SavePropertyInStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
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
        IProperty property = objectData.Property(propertyName);

        IPropertyStorageRevisionSlot propertyStorageRevisionSlot =
            fsObjectDataStorageManager.GetNextPropertyStorageRevisionSlot(property);

        propertyStorageRevisionSlot.SetData(property.ToRawData());

        IPropertyWriteResult result = propertyStorageRevisionSlot.Save(fsObjectDataStorageManager, property);
        
        result.PointerStorageSlot.FullName.ShouldBeEqualTo(propertyStorageRevisionSlot.FullName);
        File.Exists(result.PointerStorageSlot.FullName).ShouldBeTrue("file slot was not written");
    }
    
        
    [UnitTest]
    public async Task GetStorageForPropertyStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
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
        IProperty property = objectData.Property(propertyName);

        IPropertyStorageRevisionSlot propertyStorageRevisionSlot =
            fsObjectDataStorageManager.GetNextPropertyStorageRevisionSlot(property);
        Message.PrintLine(propertyStorageRevisionSlot.FullName);

        ISlottedStorage slottedStorage = fsObjectDataStorageManager.GetObjectStorage(propertyStorageRevisionSlot);
        IStorageSlot slot = slottedStorage.Save(property.ToRawData());
        slot.FullName.ShouldBeEqualTo(propertyStorageRevisionSlot.FullName);
        fsObjectDataStorageManager.IsSlotWritten(slot).ShouldBeTrue("slot was not written");
    }

    public ServiceRegistry ConfigureTestRegistry(ServiceRegistry serviceRegistry)
    {
        return Configure(serviceRegistry)
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        return UnitTests.ConfigureDependencies(rootPath);
    }
}