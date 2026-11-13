using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Services;
using Bam.Storage;
using Bam.Test;
using Bam.Data.Objects.Tests.TestClasses;

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
                return new object[] { retrieved != null, originalUuid, retrieved?.Uuid!, createdId, retrieved?.Id ?? 0UL };
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
                return new object[] { results.Count(), results.FirstOrDefault()?.Uuid!, data2.Uuid };
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

    [UnitTest]
    public async Task SearchByProperty()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchByProperty));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates entries and searches by Uuid",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                TestRepoData data3 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);
                repository.Create(data3);

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Uuid", data2.Uuid);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                object? foundData = result.Results.FirstOrDefault()?.Data;
                string? foundUuid = (foundData as TestRepoData)?.Uuid;
                return new object[] { result.Success, result.TotalCount, foundUuid!, data2.Uuid };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            string foundUuid = (string)results[2];
            string expectedUuid = (string)results[3];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found exactly 1 result", count == 1);
            because.ItsTrue("found correct entry by Uuid", expectedUuid.Equals(foundUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchMultiCriteria()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchMultiCriteria));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates entries and searches by multiple criteria",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Uuid", data1.Uuid)
                    .Where("Cuid", data1.Cuid);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                object? foundData = result.Results.FirstOrDefault()?.Data;
                string? foundCuid = (foundData as TestRepoData)?.Cuid;
                return new object[] { result.Success, result.TotalCount, foundCuid!, data1.Cuid };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            string foundCuid = (string)results[2];
            string expectedCuid = (string)results[3];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found exactly 1 result", count == 1);
            because.ItsTrue("found correct entry by Cuid", expectedCuid.Equals(foundCuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchAfterDelete()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchAfterDelete));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("creates, deletes, then searches",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);

                string deletedUuid = data1.Uuid;
                repository.Delete(data1);

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Uuid", deletedUuid);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("deleted entry not found in search", count == 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchNoResults()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchNoResults));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches for non-existent value",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                repository.Create(data1);

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Uuid", "non-existent-uuid-value");

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("no results found", count == 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task UpdateAPropertyToNull()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(UpdateAPropertyToNull));
        CleanDirectory(root);
        ClearedPropertyOutcome outcome = ClearAProperty(ConfigureTestRegistry(root));

        When.A<ObjectDataRepository>("updates a property to null over the file store",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) => outcome)
        .TheTest
        .ShouldPass<ClearedPropertyOutcome>((because, result) => AssertCleared(because, result))
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task EncryptedUpdateAPropertyToNull()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(EncryptedUpdateAPropertyToNull));
        CleanDirectory(root);
        ClearedPropertyOutcome outcome = ClearAProperty(ConfigureEncryptedTestRegistry(root));

        When.A<ObjectDataRepository>("updates a property to null over the encrypted file store",
            () => ConfigureEncryptedTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) => outcome)
        .TheTest
        .ShouldPass<ClearedPropertyOutcome>((because, result) => AssertCleared(because, result))
        .SoBeHappy()
        .UnlessItFailed();
    }

    // bam.data.objects#9: Update used to skip null properties, so a cleared value came back on every read.
    private static ClearedPropertyOutcome ClearAProperty(ServiceRegistry registry)
    {
        ObjectDataRepository repository = registry.Get<ObjectDataRepository>();
        IObjectDataStorageManager storage = registry.Get<IObjectDataStorageManager>();
        IObjectDataFactory factory = registry.Get<IObjectDataFactory>();

        TestRepoData neverNamed = repository.Create(new TestRepoData { Name = null!, Count = 1 });
        int neverNamedRevisions = storage.GetLatestRevisionNumber(NameOf(factory, neverNamed));

        string oldName = "named-" + Guid.NewGuid().ToString("N");
        TestRepoData created = repository.Create(new TestRepoData { Name = oldName, Count = 2, Modified = DateTime.UtcNow });
        created.Name = null!;
        created.Modified = null;
        repository.Update(created);
        int revisionsAfterClear = storage.GetLatestRevisionNumber(NameOf(factory, created));
        repository.Update(created);
        int revisionsAfterSecondUpdate = storage.GetLatestRevisionNumber(NameOf(factory, created));
        IPropertyDescriptor name = NameOf(factory, created);
        string? firstRevision = storage.ReadRevisions(factory.GetObjectData(created), name).First().Property.Decode() as string;

        TestRepoData? retrieved = repository.Retrieve<TestRepoData>(created.Id);
        TestRepoData? fromAll = repository.RetrieveAll<TestRepoData>().FirstOrDefault(item => item.Id == created.Id);
        ulong createdId = created.Id;
        TestRepoData? fromQuery = repository.Query<TestRepoData>(item => item.Id == createdId).FirstOrDefault();
        int oldNameMatches = repository.Query<TestRepoData>(new Dictionary<string, object> { { "Name", oldName } }).Count();
        // the predicate path filters what it reads back, so it found the stale value before the fix
        int oldNameScanMatches = repository.Query<TestRepoData>(item => item.Name == oldName).Count();

        return new ClearedPropertyOutcome(neverNamedRevisions, retrieved?.Name, retrieved?.Count ?? 0, retrieved?.Modified, fromAll?.Name, fromQuery?.Name,
            revisionsAfterClear, revisionsAfterSecondUpdate, firstRevision, oldName, oldNameMatches, oldNameScanMatches);
    }

    private static IPropertyDescriptor NameOf(IObjectDataFactory factory, TestRepoData data)
    {
        return new PropertyDescriptor
        {
            ObjectDataKey = factory.GetObjectData(data).GetObjectKey(),
            PropertyName = nameof(TestRepoData.Name),
            PropertyType = typeof(string)
        };
    }

    private static void AssertCleared(Because because, ClearedPropertyOutcome outcome)
    {
        because.ItsTrue("create writes no revision for a null property", outcome.NeverNamedRevisions == 0);
        because.ItsTrue("retrieve reads the cleared property back as null", outcome.Retrieved is null);
        because.ItsTrue("the other properties are kept", outcome.RetrievedCount == 2);
        because.ItsTrue("a nullable value type is cleared too", outcome.RetrievedModified is null);
        because.ItsTrue("retrieve-all reads it back as null", outcome.FromAll is null);
        because.ItsTrue("query reads it back as null", outcome.FromQuery is null);
        because.ItsTrue("clearing writes one new revision", outcome.RevisionsAfterClear == 2);
        because.ItsTrue("updating it while still null writes no further revision", outcome.RevisionsAfterSecondUpdate == 2);
        because.ItsTrue("the earlier value stays in the history", outcome.OldName.Equals(outcome.FirstRevision));
        because.ItsTrue("a query on the old value no longer finds it", outcome.OldNameMatches == 0);
        because.ItsTrue("a predicate query on the old value no longer finds it", outcome.OldNameScanMatches == 0);
    }

    [UnitTest]
    public async Task ClearAPropertyWhoseLatestRevisionCannotBeRead()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(ClearAPropertyWhoseLatestRevisionCannotBeRead));
        CleanDirectory(root);

        // bam.data.objects#10 review: an unreadable latest revision must not skip the clear.
        When.A<ObjectDataRepository>("clears a property whose latest revision can't be read",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                ServiceRegistry registry = ConfigureTestRegistry(root);
                IObjectDataStorageManager storage = registry.Get<IObjectDataStorageManager>();
                IObjectDataFactory factory = registry.Get<IObjectDataFactory>();
                TestRepoData created = repository.Create(new TestRepoData { Name = "unreadable-" + Guid.NewGuid().ToString("N"), Count = 3 });
                IPropertyDescriptor name = NameOf(factory, created);
                File.WriteAllText(storage.GetLatestPropertyStorageRevisionSlot(name).FullName!, "not a pointer");

                created.Name = null!;
                repository.Update(created);
                int revisions = storage.GetLatestRevisionNumber(name);
                TestRepoData? retrieved = repository.Retrieve<TestRepoData>(created.Id);
                return new UnreadableClearOutcome(revisions, retrieved is not null, retrieved?.Name);
            })
        .TheTest
        .ShouldPass<UnreadableClearOutcome>((because, outcome) =>
        {
            because.ItsTrue("a null revision is written over the unreadable one", outcome.Revisions == 2);
            because.ItsTrue("the object still reads back", outcome.Retrieved);
            because.ItsTrue("the property reads back as null", outcome.Name is null);
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

    [UnitTest]
    public void NotDeadlockUnderASingleThreadedSynchronizationContext()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(NotDeadlockUnderASingleThreadedSynchronizationContext));
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("runs its synchronous methods on a thread with a single-threaded synchronization context",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                IObjectDataSearchIndexer searchIndexer = registry.Get<IObjectDataSearchIndexer>();
                SynchronizationContextChecks checks = new SynchronizationContextChecks();
                using SingleThreadSynchronizationContext context = new SingleThreadSynchronizationContext();
                // Each call blocks on async work. With the continuations posted back to this
                // context's only thread, a missing ConfigureAwait(false) on any path hangs it.
                checks.Finished = context.Run(() =>
                {
                    TestRepoData created = repository.Create(new TestRepoData { Name = "on the context", Count = 1 });
                    checks.CreatedId = created.Id;
                    checks.Retrieved = repository.Retrieve<TestRepoData>(created.Id)?.Name == "on the context";
                    checks.RetrievedByUuid = repository.Retrieve<TestRepoData>(created.Uuid)?.Id == created.Id;
                    checks.RetrievedAll = repository.RetrieveAll<TestRepoData>().Any(item => item.Id == created.Id);
                    created.Count = 2;
                    checks.Updated = repository.Update(created).Count == 2;
                    checks.Queried = repository.Query<TestRepoData>(new Dictionary<string, object> { { nameof(TestRepoData.Name), "on the context" } }).Count();
                    searchIndexer.RebuildAsync<TestRepoData>().GetAwaiter().GetResult();
                    checks.Rebuilt = searchIndexer.HasIndex(typeof(TestRepoData));
                    // The scan fallback: a store with no search index.
                    Directory.Delete(Path.Combine(root, "search-index"), true);
                    checks.ScanQueried = repository.Query<TestRepoData>(new Dictionary<string, object> { { nameof(TestRepoData.Name), "on the context" } }).Count();
                    // IQueryFilter's scan path (only equality: other comparisons are bam.data.objects#15).
                    checks.FilterQueried = repository.Query<TestRepoData>((Bam.Data.QueryFilter)(Bam.Data.QueryFilter.Where(nameof(TestRepoData.Name)) == "on the context")).Count();
                    checks.Deleted = repository.Delete(created);
                }, TimeSpan.FromSeconds(30));
                return checks;
            })
        .TheTest
        .ShouldPass<SynchronizationContextChecks>((because, outcome) =>
        {
            because.ItsTrue("every call returned instead of hanging", outcome.Finished);
            because.ItsTrue("create assigned an id", outcome.CreatedId > 0);
            because.ItsTrue("retrieve by id found it", outcome.Retrieved);
            because.ItsTrue("retrieve by uuid found it", outcome.RetrievedByUuid);
            because.ItsTrue("retrieve all listed it", outcome.RetrievedAll);
            because.ItsTrue("update saved it", outcome.Updated);
            because.ItsTrue($"the dictionary query found it ({outcome.Queried})", outcome.Queried == 1);
            because.ItsTrue("the search index rebuilt", outcome.Rebuilt);
            because.ItsTrue($"the scan fallback found it ({outcome.ScanQueried})", outcome.ScanQueried == 1);
            because.ItsTrue($"the filter query's scan path found it ({outcome.FilterQueried})", outcome.FilterQueried == 1);
            because.ItsTrue("delete removed it", outcome.Deleted);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    // Written from the context's thread and read after Run returns.
    private sealed class SynchronizationContextChecks
    {
        public bool Finished { get; set; }
        public ulong CreatedId { get; set; }
        public bool Retrieved { get; set; }
        public bool RetrievedByUuid { get; set; }
        public bool RetrievedAll { get; set; }
        public bool Updated { get; set; }
        public int Queried { get; set; } = -1;
        public int FilterQueried { get; set; } = -1;
        public bool Rebuilt { get; set; }
        public int ScanQueried { get; set; } = -1;
        public bool Deleted { get; set; }
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
            .For<IObjectDataSearchIndexer>().Use<ObjectDataSearchIndexer>()
            .For<IObjectDataSearcher>().Use<ObjectDataSearcher>();
        return serviceRegistry;
    }

    private static ServiceRegistry ConfigureEncryptedTestRegistry(string root)
    {
        ServiceRegistry serviceRegistry = IntegrationTests.ConfigureDependencies(root);
        serviceRegistry
            .For<IAesKeySource>().Use(new AesKey())
            .For<IEncryptor>().Use<AesEncryptor>()
            .For<IDecryptor>().Use<AesDecryptor>();
        serviceRegistry
            .For<IObjectDecoder>().Use<JsonObjectDataEncoder>()
            .For<IPropertyWriter>().Use<PropertyWriter>()
            .For<IObjectDataStorageManager>().Use<EncryptedFsObjectDataStorageManager>();
        serviceRegistry
            .For<IObjectEncoderDecoder>().UseSingleton(serviceRegistry.Get<IObjectDecoder>())
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectDataWriter>().Use<ObjectDataWriter>()
            .For<IObjectDataIndexer>().Use<ObjectDataIndexer>()
            .For<IObjectDataDeleter>().Use<ObjectDataDeleter>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IObjectDataArchiver>().Use<ObjectDataArchiver>()
            .For<IObjectDataSearchIndexer>().Use<ObjectDataSearchIndexer>()
            .For<IObjectDataSearcher>().Use<ObjectDataSearcher>();
        return serviceRegistry;
    }

    [UnitTest]
    public async Task SearchStartsWith()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchStartsWith));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches with StartsWith operator",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                repository.Create(new TestRepoData { Name = "JohnDoe" });
                repository.Create(new TestRepoData { Name = "JohnSmith" });
                repository.Create(new TestRepoData { Name = "JaneDoe" });

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Name", "John", SearchOperator.StartsWith);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found 2 entries starting with John", count == 2);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchEndsWith()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchEndsWith));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches with EndsWith operator",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                repository.Create(new TestRepoData { Name = "JohnDoe" });
                repository.Create(new TestRepoData { Name = "JohnSmith" });
                repository.Create(new TestRepoData { Name = "JaneDoe" });

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Name", "Smith", SearchOperator.EndsWith);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found 1 entry ending with Smith", count == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchContains()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchContains));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches with Contains operator",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                repository.Create(new TestRepoData { Name = "JohnDoe" });
                repository.Create(new TestRepoData { Name = "JohnSmith" });
                repository.Create(new TestRepoData { Name = "JaneDoe" });

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Name", "Doe", SearchOperator.Contains);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found 2 entries containing Doe", count == 2);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchDoesntStartWith()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchDoesntStartWith));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches with DoesntStartWith operator",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                repository.Create(new TestRepoData { Name = "JohnDoe" });
                repository.Create(new TestRepoData { Name = "JohnSmith" });
                repository.Create(new TestRepoData { Name = "JaneDoe" });

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Name", "John", SearchOperator.DoesntStartWith);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found 1 entry not starting with John", count == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchDoesntEndWith()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchDoesntEndWith));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches with DoesntEndWith operator",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                repository.Create(new TestRepoData { Name = "JohnDoe" });
                repository.Create(new TestRepoData { Name = "JohnSmith" });
                repository.Create(new TestRepoData { Name = "JaneDoe" });

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Name", "Doe", SearchOperator.DoesntEndWith);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found 1 entry not ending with Doe", count == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task SearchDoesntContain()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SearchDoesntContain));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("searches with DoesntContain operator",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                repository.Create(new TestRepoData { Name = "JohnDoe" });
                repository.Create(new TestRepoData { Name = "JohnSmith" });
                repository.Create(new TestRepoData { Name = "JaneDoe" });

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Name", "Doe", SearchOperator.DoesntContain);

                IObjectDataSearcher searcher = ConfigureTestRegistry(root).Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found 1 entry not containing Doe", count == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task EncryptedCreateAndRetrieve()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(EncryptedCreateAndRetrieve));
        CleanDirectory(root);
        TestRepoData data = new TestRepoData();

        When.A<ObjectDataRepository>("creates and retrieves via encrypted storage",
            () => ConfigureEncryptedTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData created = repository.Create(data);
                TestRepoData retrieved = repository.Retrieve<TestRepoData>(created.Id);

                // Verify raw files on disk are not readable as plaintext JSON
                string rawDir = Path.Combine(root, "raw");
                bool rawFilesArePlaintext = false;
                if (Directory.Exists(rawDir))
                {
                    foreach (string file in Directory.GetFiles(rawDir, "dat", SearchOption.AllDirectories))
                    {
                        string content = File.ReadAllText(file);
                        if (content.Contains(data.Uuid))
                        {
                            rawFilesArePlaintext = true;
                            break;
                        }
                    }
                }

                return new object[] { created.Id, retrieved?.Id ?? 0UL, retrieved?.Uuid!, data.Uuid, rawFilesArePlaintext };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            ulong createdId = (ulong)results[0];
            ulong retrievedId = (ulong)results[1];
            string retrievedUuid = (string)results[2];
            string expectedUuid = (string)results[3];
            bool rawFilesArePlaintext = (bool)results[4];
            because.ItsTrue("created Id is greater than 0", createdId > 0);
            because.ItsTrue("retrieved Id equals created Id", retrievedId == createdId);
            because.ItsTrue("retrieved Uuid matches", expectedUuid.Equals(retrievedUuid));
            because.ItsTrue("raw files are NOT plaintext", !rawFilesArePlaintext);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task EncryptedSearchByProperty()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(EncryptedSearchByProperty));
        CleanDirectory(root);
        ServiceRegistry registry = ConfigureEncryptedTestRegistry(root);

        When.A<ObjectDataRepository>("creates entries and searches by Uuid via encrypted storage",
            () => registry.Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                TestRepoData data3 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);
                repository.Create(data3);

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Uuid", data2.Uuid);

                IObjectDataSearcher searcher = registry.Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                object? foundData = result.Results.FirstOrDefault()?.Data;
                string? foundUuid = (foundData as TestRepoData)?.Uuid;
                return new object[] { result.Success, result.TotalCount, foundUuid!, data2.Uuid };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            string foundUuid = (string)results[2];
            string expectedUuid = (string)results[3];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("found exactly 1 result", count == 1);
            because.ItsTrue("found correct entry by Uuid", expectedUuid.Equals(foundUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task EncryptedDeleteAndSearch()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(EncryptedDeleteAndSearch));
        CleanDirectory(root);
        ServiceRegistry registry = ConfigureEncryptedTestRegistry(root);

        When.A<ObjectDataRepository>("creates, deletes, then searches via encrypted storage",
            () => registry.Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data1 = new TestRepoData();
                TestRepoData data2 = new TestRepoData();
                repository.Create(data1);
                repository.Create(data2);

                string deletedUuid = data1.Uuid;
                repository.Delete(data1);

                ObjectDataSearch search = new ObjectDataSearch(typeof(TestRepoData))
                    .Where("Uuid", deletedUuid);

                IObjectDataSearcher searcher = registry.Get<IObjectDataSearcher>();
                IObjectDataSearchResult result = searcher.SearchAsync(search).GetAwaiter().GetResult();
                return new object[] { result.Success, result.TotalCount };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            int count = (int)results[1];
            because.ItsTrue("search succeeded", success);
            because.ItsTrue("deleted entry not found in search", count == 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record ClearedPropertyOutcome(int NeverNamedRevisions, string? Retrieved, long RetrievedCount, DateTime? RetrievedModified, string? FromAll, string? FromQuery,
        int RevisionsAfterClear, int RevisionsAfterSecondUpdate, string? FirstRevision, string OldName, int OldNameMatches, int OldNameScanMatches);

    private sealed record UnreadableClearOutcome(int Revisions, bool Retrieved, string? Name);
}
