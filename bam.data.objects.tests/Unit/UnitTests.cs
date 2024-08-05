using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Storage;

namespace Bam.Application.Unit;

public static class UnitTests
{
    public static ServiceRegistry ConfigureDependencies(string rootPath)
    {
        return new ServiceRegistry()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath));
        
    }
}