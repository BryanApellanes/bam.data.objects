using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Combines content hash calculation and composite key calculation to provide full identity resolution for object data.
/// </summary>
public interface IObjectDataIdentityCalculator : IHashCalculator, ICompositeKeyCalculator
{
    /// <summary>
    /// Gets the hash calculator used for content-based identity.
    /// </summary>
    IHashCalculator HashCalculator { get; }

    /// <summary>
    /// Gets the composite key calculator used for key-based identity.
    /// </summary>
    ICompositeKeyCalculator CompositeKeyCalculator { get; }
}