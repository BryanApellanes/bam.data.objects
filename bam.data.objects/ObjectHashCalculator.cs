using Bam.Data.Dynamic.Objects;
using Bam.Net;

namespace Bam.Data.Objects;

public class ObjectHashCalculator : IObjectHashCalculator
{
    public ObjectHashCalculator(IHashCalculator hashCalculator, IKeyHashCalculator keyHashCalculator)
    {
        Args.ThrowIfNull(hashCalculator, nameof(hashCalculator));
        Args.ThrowIfNull(keyHashCalculator, nameof(keyHashCalculator));
        
        this.HashCalculator = hashCalculator;
        this.KeyHashCalculator = keyHashCalculator;
    }
    
    public ulong CalculateHash(object instance)
    {
        return this.HashCalculator.CalculateHash(instance);
    }

    public ulong CalculateHash(IObjectData data)
    {
        return this.HashCalculator.CalculateHash(data);
    }

    public ulong CalculateKeyHash(object instance)
    {
        return this.KeyHashCalculator.CalculateKeyHash(instance);
    }

    public IHashCalculator HashCalculator { get; }
    public IKeyHashCalculator KeyHashCalculator { get; }
}