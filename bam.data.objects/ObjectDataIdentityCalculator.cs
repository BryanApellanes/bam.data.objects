using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataIdentityCalculator"/> that delegates hash calculations and composite key calculations to the provided components.
/// </summary>
public class ObjectDataIdentityCalculator : IObjectDataIdentityCalculator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataIdentityCalculator"/> class with default <see cref="JsonHashCalculator"/> and <see cref="CompositeKeyCalculator"/>.
    /// </summary>
    public ObjectDataIdentityCalculator() : this(new JsonHashCalculator(), new CompositeKeyCalculator())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataIdentityCalculator"/> class with the specified calculators.
    /// </summary>
    /// <param name="hashCalculator">The hash calculator for content-based identity.</param>
    /// <param name="compositeKeyCalculator">The composite key calculator for key-based identity.</param>
    public ObjectDataIdentityCalculator(IHashCalculator hashCalculator, ICompositeKeyCalculator compositeKeyCalculator)
    {
        Args.ThrowIfNull(hashCalculator, nameof(hashCalculator));
        Args.ThrowIfNull(compositeKeyCalculator, nameof(compositeKeyCalculator));
        
        this.HashCalculator = hashCalculator;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    /// <inheritdoc />
    public IHashCalculator HashCalculator { get; }

    /// <inheritdoc />
    public ICompositeKeyCalculator CompositeKeyCalculator { get; }

    /// <inheritdoc />
    public ulong CalculateULongHash(object instance)
    {
        return this.HashCalculator.CalculateULongHash(instance);
    }

    /// <inheritdoc />
    public ulong CalculateULongHash(IObjectData data)
    {
        return this.HashCalculator.CalculateULongHash(data);
    }

    /// <inheritdoc />
    public string CalculateHashHex(object data)
    {
        return this.HashCalculator.CalculateHashHex(data);
    }

    /// <inheritdoc />
    public string CalculateHashHex(IObjectData data)
    {
        return this.HashCalculator.CalculateHashHex(data);
    }

    /// <inheritdoc />
    public ulong CalculateULongKey(object instance)
    {
        return this.CompositeKeyCalculator.CalculateULongKey(instance);
    }

    /// <inheritdoc />
    public ulong CalculateULongKey(IObjectData objectData)
    {
        return this.CompositeKeyCalculator.CalculateULongKey(objectData);
    }

    /// <inheritdoc />
    public string CalculateHashHexKey(object instance)
    {
        return this.CompositeKeyCalculator.CalculateHashHexKey(instance);
    }

    /// <inheritdoc />
    public string CalculateHashHexKey(IObjectData objectData)
    {
        return this.CompositeKeyCalculator.CalculateHashHexKey(objectData);
    }
}