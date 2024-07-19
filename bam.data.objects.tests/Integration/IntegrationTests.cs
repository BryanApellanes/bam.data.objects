using Bam.Data.Dynamic.Objects;
using Bam.CoreServices;
using Bam.Storage;

namespace Bam.Data.Objects.Tests.Integration;

public static class IntegrationTests
{
    public static ServiceRegistry ConfigureDependencies(string rootPath)
    {
        return new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataIdentifierFactory>().Use<ObjectDataIdentifierFactory>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IRootStorageHolder>().Use( new RootStorageHolder(rootPath))
            .For<IObjectDataReader>().Use<ObjectDataReader>()
            .For<IStorageIdentifier>().Use(new FsStorageHolder(rootPath));
    }
}