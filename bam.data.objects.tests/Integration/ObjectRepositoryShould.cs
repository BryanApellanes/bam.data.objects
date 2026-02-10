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
        TestRepoData data = new TestRepoData();

        When.A<ObjectDataRepository>("creates a TestRepoData entry",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) => repository.Create(data))
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is the same reference as input", ReferenceEquals(because.Result, data));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task Retrieve()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Create));
        TestRepoData data = new TestRepoData();

        When.A<ObjectDataRepository>("creates and retrieves a TestRepoData entry",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData result = repository.Create(data);
                TestRepoData retrieved = repository.Retrieve<TestRepoData>(result.Id);
                return new object[] { result.Id, retrieved?.Id ?? 0UL };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            ulong createdId = (ulong)results[0];
            ulong retrievedId = (ulong)results[1];
            because.ItsTrue("created Id is greater than 0", createdId > 0);
            because.ItsTrue("retrieved Id equals created Id", retrievedId == createdId);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task RetrieveByUuid()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Create));
        TestRepoData data = new TestRepoData();
        string originalUuid = data.Uuid;

        When.A<ObjectDataRepository>("creates and retrieves a TestRepoData entry by UUID",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData result = repository.Create(data);
                TestRepoData retrieved = repository.Retrieve<TestRepoData>(originalUuid);
                return new object[] { retrieved, result.Id };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            TestRepoData retrieved = (TestRepoData)results[0];
            ulong createdId = (ulong)results[1];
            because.ItsTrue("retrieved is not null", retrieved != null);
            because.ItsTrue("retrieved Uuid equals original", originalUuid.Equals(retrieved?.Uuid));
            because.ItsTrue("retrieved Id equals created Id", retrieved?.Id == createdId);
        })
        .SoBeHappy()
        .UnlessItFailed();
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
