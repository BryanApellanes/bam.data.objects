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
    
    // Objects
    // {root}/objects/name/space/type/key/{Key}.dat -> {ObjectIdentifierHash}
    
    // Object properties
    // {root}/objects/name/space/type/hash/{ObjectIdentifierHash}/{propertyName}/{version}/val.dat content -> {RawDataHash}
    
    // Raw data
    // {root}/raw/{hash}.dat   
    
    [UnitTest]
    public void GetRootStorage()
    {
        string expected = Path.Combine(Environment.CurrentDirectory, nameof(GetRootStorage));
        DependencyProvider dependencyProvider = ConfigureDependencies(expected);
        ObjectStorageManager storageManager = dependencyProvider.Get<ObjectStorageManager>();

        string actual = storageManager.GetRootStorageContainer().FullName;
        actual.ShouldEqual(expected, $"root was not expected value {expected} but {actual}");
    }
    
    [UnitTest]
    public void GetTypeDirectoryForDynamicType()
    {        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        ObjectStorageManager ofs = dependencyProvider.Get<ObjectStorageManager>();
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
        ObjectStorageManager objectStorageManager = dependencyProvider.Get<ObjectStorageManager>();
        TestData ob = new TestData();
        
        IStorageIdentifier directoryInfo = objectStorageManager.GetTypeStorageContainer(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetKeyStorageIdentifier()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(GetTypeDirectoryForDynamicType));
        DependencyProvider dependencyProvider = ConfigureDependencies(root);
        ObjectStorageManager objectStorageManager = dependencyProvider.Get<ObjectStorageManager>();
        
        IObjectKey mockKey = Substitute.For<IObjectKey>();
        ulong testKey = 32.RandomLetters().ToHashULong(HashAlgorithms.SHA256);
        mockKey.Key.Returns(testKey);
        mockKey.Type.Returns(new TypeDescriptor(typeof(TestData)));
        
        IStorageIdentifier keyStorageIdentifier = objectStorageManager.GetKeyStorageContainer(mockKey);

        List<string> parts = new List<string> { root };
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
        ObjectStorageManager objectStorageManager = dependencyProvider.Get<ObjectStorageManager>();
        
        IObjectKey mockKey = Substitute.For<IObjectKey>();
        ulong testKey = 32.RandomLetters().ToHashULong(HashAlgorithms.SHA256);
        mockKey.Key.Returns(testKey);
        mockKey.Type.Returns(new TypeDescriptor(typeof(TestData)));
        
        List<string> parts = new List<string> { root };
        parts.AddRange(typeof(TestData).Namespace.Split('.'));
        parts.Add(nameof(TestData));
        parts.Add("key");
        parts.AddRange(testKey.ToString().Split(2));
        
        string expected = Path.Combine(parts.ToArray());
        
        IStorageContainer keyStorageIdentifier = objectStorageManager.GetKeyStorageContainer(mockKey);

        IStorage keyStorage = objectStorageManager.GetStorage(keyStorageIdentifier);
        keyStorage.Identifier.FullName.ShouldEqual(expected);
    }
    
    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>();
    }
    
    private DependencyProvider ConfigureDependencies(string expected)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IStorageIdentifier>().Use(new FsStorageIdentifier(expected));

        DependencyProvider dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}