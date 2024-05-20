using Bam.Console;
using Bam.Data.Objects;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using bam.data.objects;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Net.Data.Repositories;
using Bam.Net.Incubation;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

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

        IPropertyHolder property =
            fsObjectStorageManager.GetPropertyStorageHolder(objectData.Property(propertyName));
        property.TypeStorageHolder.ShouldNotBeNull("TypeStorageHolder was null");
        
        property.PropertyName.ShouldNotBeNull();
        property.PropertyName.ShouldEqual(propertyName);
        property.FullName.ShouldEqual(expected);
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
        return UnitTests.ConfigureDependencies(rootPath);
    }
}