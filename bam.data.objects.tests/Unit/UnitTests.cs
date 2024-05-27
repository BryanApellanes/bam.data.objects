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
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IObjectIdentifierFactory>().Use<ObjectIdentifierFactory>()
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath));
        
    }
}