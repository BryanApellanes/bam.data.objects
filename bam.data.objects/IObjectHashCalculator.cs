using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectHashCalculator : IHashCalculator, IKeyHashCalculator
{
    IHashCalculator HashCalculator { get; }
    IKeyHashCalculator KeyHashCalculator { get; }
}