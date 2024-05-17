using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Net.Data.Repositories;
using Bam.Net.Incubation;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("Unit: ObjectDataWriter should")]
public class ObjectDataWriterShould: UnitTestMenuContainer
{
    public ObjectDataWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task CallObjectIdentifierGetObjectKey()
    {
        IObjectIdentifierFactory mockObjectIdentifierFactory = Substitute.For<IObjectIdentifierFactory>();
        IObjectStorageManager mockStorageManager = Substitute.For<IObjectStorageManager>();
        
        ServiceRegistry testContainer = new ServiceRegistry()
            .For<IObjectStorageManager>().Use(mockStorageManager)
            .For<IObjectIdentifierFactory>().Use(mockObjectIdentifierFactory);

        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();

        TestData testData = new TestData();
        ObjectData objectData = new ObjectData(testData);
        await objectDataWriter.WriteAsync(objectData);
        mockObjectIdentifierFactory.Received().GetObjectKey(objectData);
    }

    [UnitTest]
    public async Task WriteKeyFile()
    {    
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteKeyFile));
        ServiceRegistry testContainer = ConfigureDependencies(root);
        
        IObjectKey mockKey = Substitute.For<IObjectKey>();
        string testKey = 32.RandomLetters().HashHexString(HashAlgorithms.SHA256);
        mockKey.Key.Returns(testKey);
        mockKey.Type.Returns(new TypeDescriptor(typeof(TestData)));
        mockKey.Id.Returns(testKey);
        
        IObjectDataFactory mockDataFactory = Substitute.For<IObjectDataFactory>();
        mockDataFactory.GetObjectKey(Arg.Any<IObjectData>()).Returns(mockKey);
        
        IObjectIdentifier mockObjectIdentifier = Substitute.For<IObjectIdentifier>();
        mockObjectIdentifier.Id.Returns(testKey);
        mockDataFactory.GetObjectIdentifier(Arg.Any<IObjectData>()).Returns(mockObjectIdentifier);
        
        testContainer.For<IObjectDataFactory>().Use(mockDataFactory);
        testContainer.For<IObjectStorageManager>().Use<FsObjectStorageManager>();
        testContainer.For<IObjectPropertyWriter>().Use<ObjectPropertyWriter>();
        
        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();

        TestData testData = new TestData();
        ObjectData objectData = new ObjectData(testData);
        IObjectDataWriteResult result = await objectDataWriter.WriteAsync(objectData);
        result.ObjectData.ShouldNotBeNull("result.Data was null");
        result.ObjectData.ShouldBe(objectData);
        
        List<string> parts = new List<string> { root };
        parts.Add("objects");
        parts.AddRange(typeof(TestData).Namespace.Split('.'));
        parts.Add(nameof(TestData));
        parts.AddRange(testKey.ToString().Split(2));
        parts.Add("key");
        
        string expected = Path.Combine(parts.ToArray());
        File.Exists(expected).ShouldBeTrue($"Key file was not written to the expected path {expected}");
        string keyFileContent = await File.ReadAllTextAsync(expected);
        keyFileContent.ShouldBeEqualTo(testKey.ToString());
    }


    
    [UnitTest]
    public async Task CallObjectStorageManagerGetRootStorage()
    {
        IObjectStorageManager mockStorageManager = Substitute.For<IObjectStorageManager>();
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IObjectStorageManager>().Use(mockStorageManager);

        ObjectDataWriter objectDataWriter = testRegistry.Get<ObjectDataWriter>();
        await objectDataWriter.WriteAsync(new ObjectData(new TestData()));
        mockStorageManager.Received().GetRootStorageContainer();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IRootStorageContainer>().Use( new RootStorageContainer(rootPath))
            .For<IStorageIdentifier>().Use(new FsStorageContainer(rootPath));

        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}