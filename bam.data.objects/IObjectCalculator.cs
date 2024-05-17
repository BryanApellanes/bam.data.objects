using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectCalculator : IHashCalculator, IKeyCalculator
{
    IHashCalculator HashCalculator { get; }
    IKeyCalculator KeyCalculator { get; }
}