using Bam.Data.Objects;
using Bam.Net;

namespace Bam.Data.Dynamic.Objects;

public class ObjectHashCalculator : IObjectHashCalculator
{
    public ObjectHashCalculator(IHashCalculator hashCalculator, IKeyHashCalculator keyHashCalculator)
    {
        Args.ThrowIfNull(hashCalculator, nameof(hashCalculator));
        Args.ThrowIfNull(keyHashCalculator, nameof(keyHashCalculator));
        
        this.HashCalculator = hashCalculator;
        this.KeyHashCalculator = keyHashCalculator;
    }
    
    public string CalculateHash(object instance)
    {
        return this.HashCalculator.CalculateHash(instance);
    }

    public string CalculateKeyHash(object instance)
    {
        return this.KeyHashCalculator.CalculateKeyHash(instance);
    }

    public IHashCalculator HashCalculator { get; }
    public IKeyHashCalculator KeyHashCalculator { get; }
}