using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("SearchIndexQueryShould")]
public class SearchIndexQueryShould : UnitTestMenuContainer
{
    public SearchIndexQueryShould() : base()
    {
        Configure(svcRegistry => svcRegistry.CopyFrom(BamConsoleContext.GetDefaultServiceRegistry()));
    }

    [UnitTest]
    public async Task ReturnSameResultsAsScanPathForDictionaryQuery()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(ReturnSameResultsAsScanPathForDictionaryQuery));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("queries by dictionary through the search index and by predicate scan",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData target = new TestRepoData { Name = "index-target" };
                repository.Create(target);
                repository.Create(new TestRepoData { Name = "other-one" });
                repository.Create(new TestRepoData { Name = "other-two" });

                List<TestRepoData> indexed = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "index-target" }
                }).ToList();
                List<TestRepoData> scanned = repository.Query<TestRepoData>(item => item.Name == "index-target").ToList();

                return new EquivalenceOutcome(
                    indexed.Count,
                    scanned.Count,
                    indexed.FirstOrDefault()?.Uuid,
                    scanned.FirstOrDefault()?.Uuid,
                    target.Uuid);
            })
        .TheTest
        .ShouldPass<EquivalenceOutcome>((because, outcome) =>
        {
            because.ItsTrue("indexed query returned 1 entry", outcome.IndexedCount == 1);
            because.ItsTrue("scan query returned 1 entry", outcome.ScannedCount == 1);
            because.ItsTrue("indexed result is the target", outcome.TargetUuid.Equals(outcome.IndexedUuid));
            because.ItsTrue("indexed and scan results agree", string.Equals(outcome.IndexedUuid, outcome.ScannedUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task ReturnSameResultsAsScanPathForFilterQuery()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(ReturnSameResultsAsScanPathForFilterQuery));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("queries by IQueryFilter through the search index",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData target = new TestRepoData { Name = "filter-target" };
                repository.Create(target);
                repository.Create(new TestRepoData { Name = "filter-other" });

                List<TestRepoData> filtered = repository.Query<TestRepoData>(
                    (Bam.Data.QueryFilter)(Bam.Data.QueryFilter.Where(nameof(TestRepoData.Name)) == "filter-target")).ToList();

                return new FilterQueryOutcome(filtered.Count, filtered.FirstOrDefault()?.Uuid, target.Uuid);
            })
        .TheTest
        .ShouldPass<FilterQueryOutcome>((because, outcome) =>
        {
            because.ItsTrue("filter query returned 1 entry", outcome.Count == 1);
            because.ItsTrue("filter result is the target", outcome.TargetUuid.Equals(outcome.ResultUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task NotReturnEntriesForSupersededValuesAfterUpdate()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(NotReturnEntriesForSupersededValuesAfterUpdate));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("updates an entry then queries for its old and new values",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData data = new TestRepoData { Name = "original-value" };
                repository.Create(data);
                data.Name = "rotated-value";
                repository.Update(data);

                int oldValueCount = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "original-value" }
                }).Count();
                List<TestRepoData> newValueResults = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "rotated-value" }
                }).ToList();

                return new UpdateReindexOutcome(oldValueCount, newValueResults.Count, newValueResults.FirstOrDefault()?.Uuid, data.Uuid);
            })
        .TheTest
        .ShouldPass<UpdateReindexOutcome>((because, outcome) =>
        {
            because.ItsTrue("query for the superseded value returns nothing", outcome.OldValueCount == 0);
            because.ItsTrue("query for the current value returns 1 entry", outcome.NewValueCount == 1);
            because.ItsTrue("current-value result is the updated entry", outcome.ExpectedUuid.Equals(outcome.NewValueUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task FilterStaleIndexEntriesByLiveValues()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(FilterStaleIndexEntriesByLiveValues));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("queries a value whose index entry is stale",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData data = new TestRepoData { Name = "current-state" };
                repository.Create(data);

                // Plant a stale search-index entry the way pre-reindex updates left them:
                // index a same-key snapshot carrying a value the stored object no longer has.
                TestRepoData staleSnapshot = new TestRepoData { Name = "stale-state" };
                staleSnapshot.Uuid = data.Uuid;
                staleSnapshot.Cuid = data.Cuid;
                IObjectDataFactory factory = registry.Get<IObjectDataFactory>();
                IObjectDataSearchIndexer searchIndexer = registry.Get<IObjectDataSearchIndexer>();
                searchIndexer.IndexAsync(factory.GetObjectData(staleSnapshot)).GetAwaiter().GetResult();

                int staleValueCount = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "stale-state" }
                }).Count();
                int liveValueCount = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "current-state" }
                }).Count();

                return new StaleEntryOutcome(staleValueCount, liveValueCount);
            })
        .TheTest
        .ShouldPass<StaleEntryOutcome>((because, outcome) =>
        {
            because.ItsTrue("stale index entry does not surface the object", outcome.StaleValueCount == 0);
            because.ItsTrue("live value still resolves the object", outcome.LiveValueCount == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task RemoveOnlyChangedValueEntriesOnReindex()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(RemoveOnlyChangedValueEntriesOnReindex));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("updates one of two entries sharing an indexed value",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData changing = new TestRepoData { Name = "shared-value" };
                TestRepoData keeping = new TestRepoData { Name = "shared-value" };
                repository.Create(changing);
                repository.Create(keeping);

                changing.Name = "changed-value";
                repository.Update(changing);

                List<TestRepoData> sharedResults = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "shared-value" }
                }).ToList();
                List<TestRepoData> changedResults = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "changed-value" }
                }).ToList();

                return new PartialReindexOutcome(
                    sharedResults.Count,
                    sharedResults.FirstOrDefault()?.Uuid,
                    keeping.Uuid,
                    changedResults.Count,
                    changedResults.FirstOrDefault()?.Uuid,
                    changing.Uuid);
            })
        .TheTest
        .ShouldPass<PartialReindexOutcome>((because, outcome) =>
        {
            because.ItsTrue("shared value still resolves the unchanged entry", outcome.SharedCount == 1);
            because.ItsTrue("shared-value result is the unchanged entry", outcome.KeepingUuid.Equals(outcome.SharedUuid));
            because.ItsTrue("changed value resolves the updated entry", outcome.ChangedCount == 1);
            because.ItsTrue("changed-value result is the updated entry", outcome.ChangingUuid.Equals(outcome.ChangedUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task FallBackToScanForNullValuedCriteria()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(FallBackToScanForNullValuedCriteria));
        CleanDirectory(root);

        When.A<ObjectDataRepository>("queries for a null property value",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData unnamed = new TestRepoData();
                repository.Create(unnamed);
                repository.Create(new TestRepoData { Name = "named-entry" });

                List<TestRepoData> results = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), null! }
                }).ToList();

                return new NullCriterionOutcome(results.Count, results.FirstOrDefault()?.Uuid, unnamed.Uuid);
            })
        .TheTest
        .ShouldPass<NullCriterionOutcome>((because, outcome) =>
        {
            because.ItsTrue("null-valued query returned 1 entry", outcome.Count == 1);
            because.ItsTrue("null-valued query matched the unnamed entry", outcome.ExpectedUuid.Equals(outcome.ResultUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task FallBackToScanWhenSearchIndexIsAbsent()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(FallBackToScanWhenSearchIndexIsAbsent));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("queries a legacy store whose search index directory is missing",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData target = new TestRepoData { Name = "legacy-target" };
                repository.Create(target);

                IObjectDataSearchIndexer searchIndexer = registry.Get<IObjectDataSearchIndexer>();
                bool hadIndex = searchIndexer.HasIndex(typeof(TestRepoData));

                // Simulate a store written before search indexing existed.
                string searchIndexDirectory = Path.Combine(root, "search-index");
                if (Directory.Exists(searchIndexDirectory))
                {
                    Directory.Delete(searchIndexDirectory, true);
                }

                bool hasIndexAfterDelete = searchIndexer.HasIndex(typeof(TestRepoData));
                List<TestRepoData> results = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "legacy-target" }
                }).ToList();

                return new LegacyStoreOutcome(hadIndex, hasIndexAfterDelete, results.Count, results.FirstOrDefault()?.Uuid, target.Uuid);
            })
        .TheTest
        .ShouldPass<LegacyStoreOutcome>((because, outcome) =>
        {
            because.ItsTrue("index existed after create", outcome.HadIndex);
            because.ItsTrue("index reported absent after delete", !outcome.HasIndexAfterDelete);
            because.ItsTrue("scan fallback found the entry", outcome.Count == 1);
            because.ItsTrue("fallback result is the target", outcome.TargetUuid.Equals(outcome.ResultUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task IndexCurrentStateWhenReindexingWithoutPriorState()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(IndexCurrentStateWhenReindexingWithoutPriorState));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("reindexes with a null previous state",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData data = new TestRepoData { Name = "no-prior-state" };
                repository.Create(data);

                IObjectDataFactory factory = registry.Get<IObjectDataFactory>();
                IObjectDataSearchIndexer searchIndexer = registry.Get<IObjectDataSearchIndexer>();
                IObjectDataSearchIndexResult result = searchIndexer
                    .ReindexAsync(null, factory.GetObjectData(data)).GetAwaiter().GetResult();

                int count = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "no-prior-state" }
                }).Count();

                return new NullPreviousOutcome(result.Success, count);
            })
        .TheTest
        .ShouldPass<NullPreviousOutcome>((because, outcome) =>
        {
            because.ItsTrue("reindex without prior state succeeded", outcome.Success);
            because.ItsTrue("current value resolves the entry", outcome.Count == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task EngageTheIndexRatherThanSilentlyScanning()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(EngageTheIndexRatherThanSilentlyScanning));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("queries after the target's specific index entry file is removed",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData target = new TestRepoData { Name = "engaged-target" };
                repository.Create(target);

                IObjectDataSearchIndexer searchIndexer = registry.Get<IObjectDataSearchIndexer>();
                bool typeIndexed = searchIndexer.HasIndex(typeof(TestRepoData));
                bool propertyIndexed = searchIndexer.HasIndex(typeof(TestRepoData), nameof(TestRepoData.Name));
                bool absentPropertyIndexed = searchIndexer.HasIndex(typeof(TestRepoData), "NoSuchProperty");

                // Remove exactly the target value's index entry file. Under the
                // index-authoritative contract the indexed query must now MISS while the scan
                // path still matches — if the query silently scanned, the two would agree and
                // this test could never fail.
                string valueHash = JsonObjectDataEncoder.Default.Encode("engaged-target").ToString()!
                    .HashHexString(HashAlgorithms.SHA256);
                string entryPath = Path.Combine(
                    root, "search-index", "Bam", "Data", "Dynamic", "TestClasses", "TestRepoData",
                    nameof(TestRepoData.Name), valueHash);
                bool entryExisted = File.Exists(entryPath);
                File.Delete(entryPath);

                int indexedCount = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "engaged-target" }
                }).Count();
                int scannedCount = repository.Query<TestRepoData>(item => item.Name == "engaged-target").Count();

                return new EngagementOutcome(typeIndexed, propertyIndexed, absentPropertyIndexed, entryExisted, indexedCount, scannedCount);
            })
        .TheTest
        .ShouldPass<EngagementOutcome>((because, outcome) =>
        {
            because.ItsTrue("the type has an index after create", outcome.TypeIndexed);
            because.ItsTrue("the queried property has an index directory", outcome.PropertyIndexed);
            because.ItsTrue("an absent property reports no index", !outcome.AbsentPropertyIndexed);
            because.ItsTrue("the value's index entry file existed before deletion", outcome.EntryExisted);
            because.ItsTrue("indexed query missed after the entry was removed (index engaged)", outcome.IndexedCount == 0);
            because.ItsTrue("scan path still matches the row", outcome.ScannedCount == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task RemoveOldValueIndexEntriesOnUpdate()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(RemoveOldValueIndexEntriesOnUpdate));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("inspects the raw index after an update",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData data = new TestRepoData { Name = "lookup-original" };
                repository.Create(data);
                data.Name = "lookup-rotated";
                repository.Update(data);

                IObjectDataSearchIndexer searchIndexer = registry.Get<IObjectDataSearchIndexer>();
                string oldValueHash = JsonObjectDataEncoder.Default.Encode("lookup-original").ToString()!
                    .HashHexString(HashAlgorithms.SHA256);
                string newValueHash = JsonObjectDataEncoder.Default.Encode("lookup-rotated").ToString()!
                    .HashHexString(HashAlgorithms.SHA256);
                int oldEntryCount = searchIndexer
                    .LookupAsync(typeof(TestRepoData), nameof(TestRepoData.Name), oldValueHash)
                    .GetAwaiter().GetResult().Count();
                int newEntryCount = searchIndexer
                    .LookupAsync(typeof(TestRepoData), nameof(TestRepoData.Name), newValueHash)
                    .GetAwaiter().GetResult().Count();

                return new RawIndexOutcome(oldEntryCount, newEntryCount);
            })
        .TheTest
        .ShouldPass<RawIndexOutcome>((because, outcome) =>
        {
            because.ItsTrue("the superseded value has NO raw index entries", outcome.OldEntryCount == 0);
            because.ItsTrue("the current value has exactly one raw index entry", outcome.NewEntryCount == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task FallBackToScanForUnindexableCriterionTypes()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(FallBackToScanForUnindexableCriterionTypes));
        CleanDirectory(root);
        DateTime created = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        When.A<ObjectDataRepository>("queries with a nullable-DateTime criterion",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData target = new TestRepoData { Name = "dated-target", Created = created };
                repository.Create(target);
                repository.Create(new TestRepoData { Name = "other-row", Created = created.AddDays(1) });

                // Created is DateTime? — never index-enumerable — so this must fall back to the
                // scan path and still return the matching row instead of a silent wrong-empty.
                List<TestRepoData> byCreated = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Created), created }
                }).ToList();
                List<TestRepoData> mixed = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Name), "dated-target" },
                    { nameof(TestRepoData.Created), created }
                }).ToList();

                return new UnindexableOutcome(
                    byCreated.Count,
                    byCreated.FirstOrDefault()?.Uuid,
                    mixed.Count,
                    mixed.FirstOrDefault()?.Uuid,
                    target.Uuid);
            })
        .TheTest
        .ShouldPass<UnindexableOutcome>((because, outcome) =>
        {
            because.ItsTrue("nullable-DateTime criterion returned the matching row", outcome.ByCreatedCount == 1);
            because.ItsTrue("nullable-DateTime result is the target", outcome.TargetUuid.Equals(outcome.ByCreatedUuid));
            because.ItsTrue("mixed indexable+unindexable criteria returned the matching row", outcome.MixedCount == 1);
            because.ItsTrue("mixed-criteria result is the target", outcome.TargetUuid.Equals(outcome.MixedUuid));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task CoerceCriterionValuesToThePropertyTypeOnBothPaths()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(CoerceCriterionValuesToThePropertyTypeOnBothPaths));
        CleanDirectory(root);
        DateTime created = new DateTime(2026, 2, 20, 8, 0, 0, DateTimeKind.Utc);

        When.A<ObjectDataRepository>("queries a long property with an int criterion",
            () => ConfigureTestRegistry(root).Get<ObjectDataRepository>(),
            (repository) =>
            {
                TestRepoData target = new TestRepoData { Name = "counted-target", Count = 42L, Created = created };
                repository.Create(target);
                repository.Create(new TestRepoData { Name = "counted-other", Count = 7L, Created = created });

                // int 42 against the long Count property: indexed path (coerced before hashing)
                int indexedCount = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Count), 42 }
                }).Count();

                // adding the unindexable Created criterion forces the SCAN path: coercion must
                // align it with the indexed result (CLR Equals(42L, 42) alone is false)
                int scannedCount = repository.Query<TestRepoData>(new Dictionary<string, object>
                {
                    { nameof(TestRepoData.Count), 42 },
                    { nameof(TestRepoData.Created), created }
                }).Count();

                return new CoercionOutcome(indexedCount, scannedCount);
            })
        .TheTest
        .ShouldPass<CoercionOutcome>((because, outcome) =>
        {
            because.ItsTrue("indexed path matched the int criterion against the long property", outcome.IndexedCount == 1);
            because.ItsTrue("scan path agrees with the indexed path", outcome.ScannedCount == 1);
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
            .For<IObjectDataSearchIndexer>().Use<ObjectDataSearchIndexer>()
            .For<IObjectDataSearcher>().Use<ObjectDataSearcher>();
        return serviceRegistry;
    }

    private sealed record EquivalenceOutcome(int IndexedCount, int ScannedCount, string? IndexedUuid, string? ScannedUuid, string TargetUuid);

    private sealed record FilterQueryOutcome(int Count, string? ResultUuid, string TargetUuid);

    private sealed record UpdateReindexOutcome(int OldValueCount, int NewValueCount, string? NewValueUuid, string ExpectedUuid);

    private sealed record StaleEntryOutcome(int StaleValueCount, int LiveValueCount);

    private sealed record PartialReindexOutcome(int SharedCount, string? SharedUuid, string KeepingUuid, int ChangedCount, string? ChangedUuid, string ChangingUuid);

    private sealed record NullCriterionOutcome(int Count, string? ResultUuid, string ExpectedUuid);

    private sealed record LegacyStoreOutcome(bool HadIndex, bool HasIndexAfterDelete, int Count, string? ResultUuid, string TargetUuid);

    private sealed record NullPreviousOutcome(bool Success, int Count);

    private sealed record EngagementOutcome(bool TypeIndexed, bool PropertyIndexed, bool AbsentPropertyIndexed, bool EntryExisted, int IndexedCount, int ScannedCount);

    private sealed record RawIndexOutcome(int OldEntryCount, int NewEntryCount);

    private sealed record UnindexableOutcome(int ByCreatedCount, string? ByCreatedUuid, int MixedCount, string? MixedUuid, string TargetUuid);

    private sealed record CoercionOutcome(int IndexedCount, int ScannedCount);
}
