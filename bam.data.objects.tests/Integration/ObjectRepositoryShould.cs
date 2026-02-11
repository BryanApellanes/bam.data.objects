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

    [UnitTest]
    public async Task Delete()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Delete));
        CleanDirectory(root);
        TestRepoData data = new TestRepoData();

        When.A<ObjectDataRepository>("creates and deletes a TestRepoData entry",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData created = repository.Create(data);
                ulong createdId = created.Id;
                bool deleted = repository.Delete(created);
                TestRepoData retrieved = repository.Retrieve<TestRepoData>(createdId);
                return new object[] { deleted, retrieved == null };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool deleted = (bool)results[0];
            bool retrievedIsNull = (bool)results[1];
            because.ItsTrue("delete returned true", deleted);
            because.ItsTrue("retrieve after delete returns null", retrievedIsNull);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task CreateNonGeneric()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(CreateNonGeneric));
        TestRepoData data = new TestRepoData();

        When.A<ObjectDataRepository>("creates a TestRepoData entry using non-generic Create",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                object result = repository.Create((object)data);
                TestRepoData created = (TestRepoData)result;
                TestRepoData retrieved = repository.Retrieve<TestRepoData>(created.Id);
                return new object[] { created.Id, retrieved?.Id ?? 0UL };
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
    public async Task Update()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(Update));
        CleanDirectory(root);
        TestRepoData data = new TestRepoData();
        string originalUuid = data.Uuid;

        When.A<ObjectDataRepository>("creates and updates a TestRepoData entry",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData created = repository.Create(data);
                ulong createdId = created.Id;
                repository.Update(created);
                TestRepoData retrieved = repository.Retrieve<TestRepoData>(createdId);
                return new object[] { retrieved != null, originalUuid, retrieved?.Uuid, createdId, retrieved?.Id ?? 0UL };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool notNull = (bool)results[0];
            string origUuid = (string)results[1];
            string retrievedUuid = (string)results[2];
            ulong createdId = (ulong)results[3];
            ulong retrievedId = (ulong)results[4];
            because.ItsTrue("retrieved is not null after update", notNull);
            because.ItsTrue("Uuid is preserved after update", origUuid.Equals(retrievedUuid));
            because.ItsTrue("Id is preserved after update", createdId == retrievedId);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task RetrieveAll()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(RetrieveAll));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates multiple entries and retrieves all",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                TestRepoData data3 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);
                repository.Create(data3);
                IEnumerable<TestRepoData> all = repository.RetrieveAll<TestRepoData>();
                return all.Count();
            })
        .TheTest
        .ShouldPass(because =>
        {
            int count = (int)because.Result;
            because.ItsTrue("retrieved all 3 entries", count == 3);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task QueryWithPredicate()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(QueryWithPredicate));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates entries and queries with predicate",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                TestRepoData data3 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);
                repository.Create(data3);
                ulong targetId = data2.Id;
                IEnumerable<TestRepoData> results = repository.Query<TestRepoData>(item => item.Id == targetId);
                return new object[] { results.Count(), results.FirstOrDefault()?.Uuid, data2.Uuid };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            int count = (int)results[0];
            string retrievedUuid = (string)results[1];
            string expectedUuid = (string)results[2];
            because.ItsTrue("query returned 1 matching entry", count == 1);
            because.ItsTrue("matched entry has correct Uuid", expectedUuid.Equals(retrievedUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task QueryWithDictionary()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(QueryWithDictionary));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates entries and queries with dictionary",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                TestRepoData data3 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);
                repository.Create(data3);
                Dictionary<string, object> queryParams = new Dictionary<string, object>
                {
                    { "Uuid", data1.Uuid }
                };
                IEnumerable<TestRepoData> results = repository.Query<TestRepoData>(queryParams);
                return new object[] { results.Count(), results.FirstOrDefault()?.Id ?? 0UL, data1.Id };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            int count = (int)results[0];
            ulong retrievedId = (ulong)results[1];
            ulong expectedId = (ulong)results[2];
            because.ItsTrue("query returned 1 matching entry", count == 1);
            because.ItsTrue("matched entry has correct Id", retrievedId == expectedId);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task BatchRetrieveAll()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(BatchRetrieveAll));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates entries and batch retrieves",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                TestRepoData data3 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);
                repository.Create(data3);
                int batchCount = 0;
                int totalItems = 0;
                repository.BatchRetrieveAll(typeof(TestRepoData), 2, batch =>
                {
                    batchCount++;
                    totalItems += batch.Count();
                });
                return new object[] { batchCount, totalItems };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            int batchCount = (int)results[0];
            int totalItems = (int)results[1];
            because.ItsTrue("received 2 batches (2+1)", batchCount == 2);
            because.ItsTrue("total items is 3", totalItems == 3);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static void CleanDirectory(string root)
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
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
