using Bam.Console;
using Bam.Data.Objects;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectStorageManager Should", "ofs")]
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

        string actual = storageManager.GetRootStorageContainer().FullName;
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
        
        DirectoryInfo directoryInfo = ofs.GetTypeStorageContainer(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetTypeDirectory()
    {        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectStorageManager fsObjectStorageManager = dependencyProvider.Get<FsObjectStorageManager>();
        TestData ob = new TestData();
        
        IStorageIdentifier directoryInfo = fsObjectStorageManager.GetTypeStorageContainer(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetKeyStorageIdentifier()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectStorageManager fsObjectStorageManager = dependencyProvider.Get<FsObjectStorageManager>();
        
        IObjectKey mockKey = Substitute.For<IObjectKey>();
        string testKey = 32.RandomLetters().HashHexString(HashAlgorithms.SHA256);
        mockKey.Key.Returns(testKey);
        mockKey.Type.Returns(new TypeDescriptor(typeof(TestData)));
        
        IStorageIdentifier keyStorageIdentifier = fsObjectStorageManager.GetKeyStorageContainer(mockKey);

        List<string> parts = new List<string> { root, "objects" };
        parts.AddRange(typeof(TestData).Namespace.Split('.'));
        parts.Add(nameof(TestData));
        parts.Add("key");
        parts.AddRange(testKey.ToString().Split(2));
        
        string expected = Path.Combine(parts.ToArray());
        keyStorageIdentifier.FullName.ShouldEqual(expected);
        Message.PrintLine(expected);
    }

    [UnitTest]
    public void GetKeyStorage()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetKeyStorage));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        FsObjectStorageManager fsObjectStorageManager = dependencyProvider.Get<FsObjectStorageManager>();
        
        IObjectKey mockKey = Substitute.For<IObjectKey>();
        string testKey = 32.RandomLetters().HashHexString(HashAlgorithms.SHA256);
        mockKey.Key.Returns(testKey);
        mockKey.Type.Returns(new TypeDescriptor(typeof(TestData)));
        
        List<string> parts = new List<string> { root,"objects" };
        parts.AddRange(typeof(TestData).Namespace.Split('.'));
        parts.Add(nameof(TestData));
        parts.Add("key");
        parts.AddRange(testKey.ToString().Split(2));
        
        string expected = Path.Combine(parts.ToArray());
        
        IStorageContainer keyStorageIdentifier = fsObjectStorageManager.GetKeyStorageContainer(mockKey);

        IStorage keyStorage = fsObjectStorageManager.GetStorage(keyStorageIdentifier);
        keyStorage.RootContainer.FullName.ShouldEqual(expected);
        Message.PrintLine(expected, ConsoleColor.Green);
    }

    [UnitTest]
    public void GetPropertyStorageContainer()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetPropertyStorageContainer));
        ServiceRegistry serviceRegistry = Configure(ConfigureDependencies(root));

        FsObjectStorageManager fsObjectStorageManager = serviceRegistry.Get<FsObjectStorageManager>();
        ObjectCalculator calculator = serviceRegistry.Get<ObjectCalculator>();
        string propertyName = "StringProperty";
        
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 64.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };
        ObjectData objectData = new ObjectData(testData);
        ulong key = calculator.CalculateULongKey(objectData);//objectData.GetKey(calculator);
        List<string> parts = new List<string> { root, "objects" };
        parts.AddRange(typeof(TestData).Namespace.Split('.'));
        parts.Add(nameof(TestData));
        parts.AddRange(key.ToString().Split(2));
        parts.Add(propertyName);
        parts.Add("1");
        string expected = Path.Combine(parts.ToArray());

        IStorageContainer propertyStorage =
            fsObjectStorageManager.GetPropertyStorageContainer(objectData.Property(propertyName));

        // {root}/objects/name/space/type/{propertyName}/{k/e/y}/1
        propertyStorage.FullName.ShouldEqual(expected);
        Message.PrintLine(expected);
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
            .For<IRootStorageContainer>().Use(new RootStorageContainer(rootPath));

        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}