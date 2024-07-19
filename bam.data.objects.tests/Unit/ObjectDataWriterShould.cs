using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Data.Repositories;
using Bam.Incubation;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Application.Unit;

[UnitTestMenu("Unit: ObjectDataWriter should")]
public class ObjectDataWriterShould: UnitTestMenuContainer
{
    public ObjectDataWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task CallObjectIdentifierGetObjectKey()
    {
        IObjectDataIdentifierFactory mockObjectDataIdentifierFactory = Substitute.For<IObjectDataIdentifierFactory>();
        IObjectDataStorageManager mockDataStorageManager = Substitute.For<IObjectDataStorageManager>();
        
        ServiceRegistry testContainer = new ServiceRegistry()
            .For<IObjectDataStorageManager>().Use(mockDataStorageManager)
            .For<IObjectDataIdentifierFactory>().Use(mockObjectDataIdentifierFactory);

        ObjectDataWriter objectDataWriter = testContainer.Get<ObjectDataWriter>();

        TestData testData = new TestData();
        ObjectData objectData = new ObjectData(testData);
        await objectDataWriter.WriteAsync(objectData);
        mockObjectDataIdentifierFactory.Received().GetObjectKey(objectData);
    }

    [UnitTest]
    public async Task WriteKeyFile()
    {
        throw new InvalidOperationException(
            "Review this test for validity.  Why should a key file be written to the expected path" +
            "This should probably be replaced by the concept of an IObjectDataIndexer");
        
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteKeyFile));
        ServiceRegistry testContainer = ConfigureDependencies(root);
        
        IObjectDataKey mockDataKey = Substitute.For<IObjectDataKey>();
        string testKey = 32.RandomLetters().HashHexString(HashAlgorithms.SHA256);
        mockDataKey.Key.Returns(testKey);
        mockDataKey.TypeDescriptor.Returns(new TypeDescriptor(typeof(TestData)));
        mockDataKey.Id.Returns(testKey);
        
        IObjectDataFactory mockDataFactory = Substitute.For<IObjectDataFactory>();
        mockDataFactory.GetObjectKey(Arg.Any<IObjectData>()).Returns(mockDataKey);
        
        IObjectDataIdentifier mockObjectDataIdentifier = Substitute.For<IObjectDataIdentifier>();
        mockObjectDataIdentifier.Id.Returns(testKey);
        mockDataFactory.GetObjectIdentifier(Arg.Any<IObjectData>()).Returns(mockObjectDataIdentifier);

        testContainer
            .For<IObjectDataReader>().Use<ObjectDataReader>()
            .For<IObjectDecoder>().Use<JsonObjectDataEncoder>()
            .For<IObjectDataFactory>().Use(mockDataFactory)
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>()
            .For<IPropertyWriter>().Use<PropertyWriter>();
        
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
        IObjectDataStorageManager mockDataStorageManager = Substitute.For<IObjectDataStorageManager>();
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataIdentifierFactory>().Use<ObjectDataIdentifierFactory>()
            .For<IObjectDataStorageManager>().Use(mockDataStorageManager);

        ObjectDataWriter objectDataWriter = testRegistry.Get<ObjectDataWriter>();
        await objectDataWriter.WriteAsync(new ObjectData(new TestData()));
        mockDataStorageManager.Received().GetRootStorageHolder();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataIdentifierFactory>().Use<ObjectDataIdentifierFactory>()
            .For<IRootStorageHolder>().Use( new RootStorageHolder(rootPath))
            .For<IStorageIdentifier>().Use(new FsStorageHolder(rootPath));

        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}