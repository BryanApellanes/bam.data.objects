using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectDataWriter should")]
public class ObjectDataWriterShould: UnitTestMenuContainer
{
    public ObjectDataWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task CallObjectIdentifierGetObjectKeyFor()
    {
        IObjectIdentifierFactory mockObjectIdentifierFactory = Substitute.For<IObjectIdentifierFactory>();
        IObjectStorageManager mockStorageManager = Substitute.For<IObjectStorageManager>();
        
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IObjectStorageManager>().Use(mockStorageManager)
            .For<IObjectIdentifierFactory>().Use(mockObjectIdentifierFactory);

        ObjectDataWriter objectDataWriter = testRegistry.Get<ObjectDataWriter>();

        TestData testData = new TestData();
        ObjectData objectData = new ObjectData(testData);
        await objectDataWriter.WriteAsync(objectData);
        mockObjectIdentifierFactory.Received().GetObjectKeyFor(objectData);
    }

    [UnitTest]
    public void WriteKeyFile()
    {    
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteKeyFile));
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
    }
    
    [UnitTest]
    public async Task CallObjectStorageManagerGetRootStorage()
    {
        IObjectStorageManager mockStorageManager = Substitute.For<IObjectStorageManager>();
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>()
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IObjectStorageManager>().Use(mockStorageManager);

        ObjectDataWriter objectDataWriter = testRegistry.Get<ObjectDataWriter>();
        await objectDataWriter.WriteAsync(new ObjectData(new TestData()));
        mockStorageManager.Received().GetRootStorageContainer();
    }
    
    private DependencyProvider ConfigureDependencies(string expected)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IStorageIdentifier>().Use(new FsStorageIdentifier(expected));

        DependencyProvider dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}