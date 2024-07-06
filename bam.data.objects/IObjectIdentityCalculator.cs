using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectIdentityCalculator : IHashCalculator, IKeyCalculator
{
    IHashCalculator HashCalculator { get; }
    IKeyCalculator KeyCalculator { get; }
}