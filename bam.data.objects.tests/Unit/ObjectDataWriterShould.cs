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
        mockObjectIdentifierFactory.Received().GetObjectKey(objectData);
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
    
    private DependencyProvider ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IStorageIdentifier>().Use(new FsStorageIdentifier(rootPath));

        DependencyProvider dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}