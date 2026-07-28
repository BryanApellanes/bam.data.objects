using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("ObjectDataArchiverShould")]
public class ObjectDataArchiverShould : UnitTestMenuContainer
{
    public ObjectDataArchiverShould() : base()
    {
        Configure(svcRegistry => svcRegistry.CopyFrom(BamConsoleContext.GetDefaultServiceRegistry()));
    }

    [UnitTest]
    public async Task RemoveArchivedObjectFromLiveStorageAndPreserveBytes()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(RemoveArchivedObjectFromLiveStorageAndPreserveBytes));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("archives a created entry",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData data = new TestRepoData { Name = "archive-me" };
                repository.Create(data);
                ulong createdId = data.Id;
                string uuid = data.Uuid;

                IObjectDataArchiver archiver = registry.Get<IObjectDataArchiver>();
                IObjectDataArchiveResult result = archiver.ArchiveAsync(data).GetAwaiter().GetResult();

                TestRepoData retrievedById = repository.Retrieve<TestRepoData>(createdId);
                TestRepoData retrievedByUuid = repository.Retrieve<TestRepoData>(uuid);
                int liveCount = repository.RetrieveAll<TestRepoData>().Count();
                bool archiveDirExists = Directory.Exists(result.ArchivePath)
                    && Directory.EnumerateFileSystemEntries(result.ArchivePath).Any();

                return new ArchiveOutcome(
                    result.Success,
                    retrievedById == null,
                    retrievedByUuid == null,
                    liveCount,
                    archiveDirExists,
                    result.ArchivePath?.StartsWith(Path.Combine(root, "archive")) == true);
            })
        .TheTest
        .ShouldPass<ArchiveOutcome>((because, outcome) =>
        {
            because.ItsTrue("archive reported success", outcome.Success);
            because.ItsTrue("retrieve by id returns null after archive", outcome.ByIdIsNull);
            because.ItsTrue("retrieve by uuid returns null after archive", outcome.ByUuidIsNull);
            because.ItsTrue("live enumeration no longer includes the object", outcome.LiveCount == 0);
            because.ItsTrue("archived bytes exist under the archive area", outcome.ArchiveDirExists);
            because.ItsTrue("archive path is under the storage root's archive area", outcome.ArchivePathUnderRoot);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task NotCollideWhenTheSameKeyIsArchivedTwice()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(NotCollideWhenTheSameKeyIsArchivedTwice));
        CleanDirectory(root);
        ServiceRegistry registry = null!;

        When.A<ObjectDataRepository>("archives the same logical entry twice",
            () =>
            {
                registry = ConfigureTestRegistry(root);
                return registry.Get<ObjectDataRepository>();
            },
            (repository) =>
            {
                TestRepoData first = new TestRepoData { Name = "first-life" };
                repository.Create(first);
                IObjectDataArchiver archiver = registry.Get<IObjectDataArchiver>();
                IObjectDataArchiveResult firstResult = archiver.ArchiveAsync(first).GetAwaiter().GetResult();

                // Re-create the same logical row (same Uuid/Cuid → same composite key) and archive again.
                TestRepoData second = new TestRepoData { Name = "second-life" };
                second.Uuid = first.Uuid;
                second.Cuid = first.Cuid;
                repository.Create(second);
                IObjectDataArchiveResult secondResult = archiver.ArchiveAsync(second).GetAwaiter().GetResult();

                return new TwiceArchivedOutcome(
                    firstResult.Success,
                    secondResult.Success,
                    firstResult.ArchivePath,
                    secondResult.ArchivePath,
                    Directory.Exists(firstResult.ArchivePath) && Directory.Exists(secondResult.ArchivePath));
            })
        .TheTest
        .ShouldPass<TwiceArchivedOutcome>((because, outcome) =>
        {
            because.ItsTrue("first archive succeeded", outcome.FirstSuccess);
            because.ItsTrue("second archive succeeded", outcome.SecondSuccess);
            because.ItsTrue("the two archives landed at distinct paths", !string.Equals(outcome.FirstPath, outcome.SecondPath));
            because.ItsTrue("both archive directories exist", outcome.BothExist);
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

    private sealed record ArchiveOutcome(bool Success, bool ByIdIsNull, bool ByUuidIsNull, int LiveCount, bool ArchiveDirExists, bool ArchivePathUnderRoot);

    private sealed record TwiceArchivedOutcome(bool FirstSuccess, bool SecondSuccess, string? FirstPath, string? SecondPath, bool BothExist);
}
