using Bam.Data.Dynamic.Objects;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;

namespace Bam.Data.Objects.Tests.Integration;

public static class IntegrationTests
{
    public static ServiceRegistry ConfigureDependencies(string rootPath)
    {
        return new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IRootStorageHolder>().Use( new RootStorageHolder(rootPath))
            .For<IObjectDataReader>().Use<ObjectDataReader>()
            .For<IStorageIdentifier>().Use(new FsStorageHolder(rootPath));
    }
}