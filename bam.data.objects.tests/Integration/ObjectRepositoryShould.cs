using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("ObjectRepositoryShould")]
public class ObjectRepositoryShould : UnitTestMenuContainer
{
    public ObjectRepositoryShould(): base()
    {
        Configure(svcRegistry => svcRegistry.CopyFrom(BamConsoleContext.GetDefaultServiceRegistry()));
    }

    [UnitTest]
    public async Task Create()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Create));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(root);

        ObjectDataRepository repository = serviceRegistry.Get<ObjectDataRepository>();
        TestRepoData data = new TestRepoData();
        TestRepoData result = repository.Create(data);
        result.ShouldBe(data);
    }
    [UnitTest]
    public async Task Retrieve()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Create));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(root);

        ObjectDataRepository repository = serviceRegistry.Get<ObjectDataRepository>();
        TestRepoData data = new TestRepoData();
        TestRepoData result = repository.Create(data);
        result.Id.ShouldBeGreaterThan(0);
        TestRepoData retrieved = repository.Retrieve<TestRepoData>(result.Id);
        retrieved.Id.ShouldEqual(result.Id);
    }

    [UnitTest]
    public async Task RetrieveByUuid()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Create));
        ServiceRegistry serviceRegistry = ConfigureTestRegistry(root);

        ObjectDataRepository repository = serviceRegistry.Get<ObjectDataRepository>();
        TestRepoData data = new TestRepoData();
        string originalUuid = data.Uuid;
        TestRepoData result = repository.Create(data);
        TestRepoData retrieved = repository.Retrieve<TestRepoData>(originalUuid);
        retrieved.ShouldNotBeNull("retrieved was null");
        retrieved.Uuid.ShouldEqual(originalUuid);
        retrieved.Id.ShouldEqual(result.Id);
    }

    private static ServiceRegistry ConfigureTestRegistry(string root)
    {
        ServiceRegistry serviceRegistry = IntegrationTests.ConfigureDependencies(root);
        serviceRegistry
            .For<IObjectDecoder>().Use<JsonObjectDataEncoder>()
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
        serviceRegistry
            .For<IObjectEncoderDecoder>().UseSingleton(serviceRegistry.Get<IObjectDecoder>())
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectDataWriter>().Use<ObjectDataWriter>()
            .For<IObjectDataIndexer>().Use<ObjectDataIndexer>()
            .For<IObjectDataDeleter>().Use<ObjectDataDeleter>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IObjectDataArchiver>().Use<ObjectDataArchiver>()
            .For<IObjectDataSearcher>().Use<ObjectDataSearcher>();
        return serviceRegistry;
    }
}