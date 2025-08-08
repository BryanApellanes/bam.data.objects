using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectDataIdentityCalculator : IHashCalculator, ICompositeKeyCalculator
{
    IHashCalculator HashCalculator { get; }
    ICompositeKeyCalculator CompositeKeyCalculator { get; }
}