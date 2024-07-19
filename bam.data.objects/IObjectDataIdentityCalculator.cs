using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectDataIdentityCalculator : IHashCalculator, IKeyCalculator
{
    IHashCalculator HashCalculator { get; }
    IKeyCalculator KeyCalculator { get; }
}