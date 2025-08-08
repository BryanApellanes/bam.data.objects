using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public class ObjectDataIdentityCalculator : IObjectDataIdentityCalculator
{
    public ObjectDataIdentityCalculator() : this(new JsonHashCalculator(), new CompositeKeyCalculator())
    {
    }

    public ObjectDataIdentityCalculator(IHashCalculator hashCalculator, ICompositeKeyCalculator compositeKeyCalculator)
    {
        Args.ThrowIfNull(hashCalculator, nameof(hashCalculator));
        Args.ThrowIfNull(compositeKeyCalculator, nameof(compositeKeyCalculator));
        
        this.HashCalculator = hashCalculator;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    public IHashCalculator HashCalculator { get; }
    public ICompositeKeyCalculator CompositeKeyCalculator { get; }
    
    public ulong CalculateULongHash(object instance)
    {
        return this.HashCalculator.CalculateULongHash(instance);
    }

    public ulong CalculateULongHash(IObjectData data)
    {
        return this.HashCalculator.CalculateULongHash(data);
    }

    public string CalculateHashHex(object data)
    {
        return this.HashCalculator.CalculateHashHex(data);
    }

    public string CalculateHashHex(IObjectData data)
    {
        return this.HashCalculator.CalculateHashHex(data);
    }

    public ulong CalculateULongKey(object instance)
    {
        return this.CompositeKeyCalculator.CalculateULongKey(instance);
    }

    public ulong CalculateULongKey(IObjectData objectData)
    {
        return this.CompositeKeyCalculator.CalculateULongKey(objectData);
    }

    public string CalculateHashHexKey(object instance)
    {
        return this.CompositeKeyCalculator.CalculateHashHexKey(instance);
    }

    public string CalculateHashHexKey(IObjectData objectData)
    {
        return this.CompositeKeyCalculator.CalculateHashHexKey(objectData);
    }
}