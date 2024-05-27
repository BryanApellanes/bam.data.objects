using Bam.Data.Dynamic.Objects;
using Bam;

namespace Bam.Data.Objects;

public class ObjectCalculator : IObjectCalculator
{
    public ObjectCalculator(IHashCalculator hashCalculator, IKeyCalculator keyCalculator)
    {
        Args.ThrowIfNull(hashCalculator, nameof(hashCalculator));
        Args.ThrowIfNull(keyCalculator, nameof(keyCalculator));
        
        this.HashCalculator = hashCalculator;
        this.KeyCalculator = keyCalculator;
    }
    
    public IHashCalculator HashCalculator { get; }
    public IKeyCalculator KeyCalculator { get; }
    
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
        return this.KeyCalculator.CalculateULongKey(instance);
    }

    public ulong CalculateULongKey(IObjectData objectData)
    {
        return this.KeyCalculator.CalculateULongKey(objectData);
    }

    public string CalculateHashHexKey(object instance)
    {
        return this.KeyCalculator.CalculateHashHexKey(instance);
    }

    public string CalculateHashHexKey(IObjectData objectData)
    {
        return this.KeyCalculator.CalculateHashHexKey(objectData);
    }
}