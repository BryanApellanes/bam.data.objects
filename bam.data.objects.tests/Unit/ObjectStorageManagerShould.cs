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

        When.A<FsObjectDataStorageManager>("gets root storage",
            () => ConfigureDependencies(expected).Get<FsObjectDataStorageManager>(),
            (mgr) => mgr.GetRootStorageHolder().FullName)
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .IsEqualTo(expected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetTypeDirectoryForDynamicType()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));

        When.A<FsObjectDataStorageManager>("gets type directory for dynamic type",
            () => ConfigureDependencies(root).Get<FsObjectDataStorageManager>(),
            (ofs) =>
            {
                dynamic ob = new { };
                return ofs.GetObjectStorageHolder(ob.GetType());
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("directory info is not null", because.Result != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetTypeStorage()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));

        When.A<FsObjectDataStorageManager>("gets type storage for PlainTestClass",
            () => ConfigureDependencies(root).Get<FsObjectDataStorageManager>(),
            (fsObjectDataStorageManager) =>
            {
                PlainTestClass ob = new PlainTestClass();
                return fsObjectDataStorageManager.GetObjectStorageHolder(ob.GetType());
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("typeStorageHolder is not null", because.Result != null);
            because.ItsTrue("typeStorageHolder has RootStorageHolder",
                because.Result is ITypeStorageHolder tsh && tsh.RootStorageHolder != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetPropertyStorageContainer()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetPropertyStorageContainer));
        string propertyName = "StringProperty";
        string expected = null;

        When.A<FsObjectDataStorageManager>("gets property storage container",
            () =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return serviceRegistry.Get<FsObjectDataStorageManager>();
            },
            (fsObjectDataStorageManager) =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
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
                parts.Add(dataKey.GetPath(fsObjectDataStorageManager));
                parts.Add(propertyName);
                expected = Path.Combine(parts.ToArray());
                return fsObjectDataStorageManager.GetPropertyStorageHolder(objectData.Property(propertyName).ToDescriptor());
            })
        .TheTest
        .ShouldPass(because =>
        {
            IPropertyStorageHolder propertyStorage = (IPropertyStorageHolder)because.Result;
            because.ItsTrue("TypeStorageHolder is not null", propertyStorage?.TypeStorageHolder != null);
            because.ItsTrue("PropertyName is not null", propertyStorage?.PropertyName != null);
            because.ItsTrue("PropertyName equals expected", propertyName.Equals(propertyStorage?.PropertyName));
            because.ItsTrue("FullName equals expected path", expected.Equals(propertyStorage?.FullName));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task GetNextPropertyStorageRevisionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetNextPropertyStorageRevisionSlot));

        When.A<FsObjectDataStorageManager>("gets next property storage revision slot",
            () =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return serviceRegistry.Get<FsObjectDataStorageManager>();
            },
            (fsObjectDataStorageManager) =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
                PlainTestClass plainTestClass = new PlainTestClass
                {
                    IntProperty = RandomNumber.Between(1, 100),
                    StringProperty = 64.RandomLetters(),
                    LongProperty = RandomNumber.Between(100, 1000),
                    DateTimeProperty = DateTime.Now
                };
                IObjectData objectData = dataFactory.GetObjectData(new ObjectData(plainTestClass));
                return fsObjectDataStorageManager.GetNextPropertyStorageRevisionSlot(objectData.Property("StringProperty"));
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("propertyStorageRevisionSlot is not null", because.Result != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SavePropertyInStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));

        When.A<FsObjectDataStorageManager>("saves a property in a storage version slot",
            () =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return serviceRegistry.Get<FsObjectDataStorageManager>();
            },
            (fsObjectDataStorageManager) =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
                PlainTestClass plainTestClass = new PlainTestClass
                {
                    IntProperty = RandomNumber.Between(1, 100),
                    StringProperty = 64.RandomLetters(),
                    LongProperty = RandomNumber.Between(100, 1000),
                    DateTimeProperty = DateTime.Now
                };
                IObjectData objectData = dataFactory.GetObjectData(new ObjectData(plainTestClass));
                IProperty property = objectData.Property("StringProperty");
                IPropertyWriteResult result = fsObjectDataStorageManager.WriteProperty(property);
                return result;
            })
        .TheTest
        .ShouldPass(because =>
        {
            IPropertyWriteResult result = (IPropertyWriteResult)because.Result;
            because.ItsTrue("PointerStorageSlot is not null", result?.PointerStorageSlot != null);
            because.ItsTrue("file slot was written", File.Exists(result?.PointerStorageSlot?.FullName));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task GetStorageForPropertyStorageVersionSlot()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SavePropertyInStorageVersionSlot));

        When.A<FsObjectDataStorageManager>("gets storage for property storage version slot",
            () =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return serviceRegistry.Get<FsObjectDataStorageManager>();
            },
            (fsObjectDataStorageManager) =>
            {
                ServiceRegistry serviceRegistry = ConfigureTestRegistry(ConfigureDependencies(root))
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                ObjectDataFactory dataFactory = serviceRegistry.Get<ObjectDataFactory>();
                PlainTestClass plainTestClass = new PlainTestClass
                {
                    IntProperty = RandomNumber.Between(1, 100),
                    StringProperty = 64.RandomLetters(),
                    LongProperty = RandomNumber.Between(100, 1000),
                    DateTimeProperty = DateTime.Now
                };
                IObjectData objectData = dataFactory.GetObjectData(new ObjectData(plainTestClass));
                IProperty property = objectData.Property("StringProperty");
                IPropertyStorageRevisionSlot propertyStorageRevisionSlot =
                    fsObjectDataStorageManager.GetNextPropertyStorageRevisionSlot(property);
                ISlottedStorage slottedStorage = fsObjectDataStorageManager.GetObjectStorage(propertyStorageRevisionSlot);
                IStorageSlot slot = slottedStorage.Save(property.ToRawData());
                return new object[] { slot, propertyStorageRevisionSlot, fsObjectDataStorageManager };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            IStorageSlot slot = (IStorageSlot)results[0];
            IPropertyStorageRevisionSlot revisionSlot = (IPropertyStorageRevisionSlot)results[1];
            FsObjectDataStorageManager mgr = (FsObjectDataStorageManager)results[2];
            because.ItsTrue("slot path equals revision slot path", slot?.FullName?.Equals(revisionSlot?.FullName) == true);
            because.ItsTrue("slot was written", mgr.IsSlotWritten(slot));
        })
        .SoBeHappy()
        .UnlessItFailed();
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
