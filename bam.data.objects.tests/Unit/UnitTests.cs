using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Application.Unit;

public static class UnitTests
{
    public static ServiceRegistry ConfigureDependencies(string rootPath)
    {
        return new ServiceRegistry()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath));
        
    }
}