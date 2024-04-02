namespace Bam.Data.Dynamic.Objects;



public class DeterministicObjectIdentifierFactory : IDeterministicObjectIdentifierFactory
{
    public DeterministicObjectIdentifierFactory(IKeyHashCalculator keyHashCalculator, IHashCalculator hashCalculator)
    {
        this.KeyHashCalculator = keyHashCalculator;
        this.HashCalculator = hashCalculator;
    }

    private IKeyHashCalculator KeyHashCalculator { get; set; }
    private IHashCalculator HashCalculator { get; set; }
    
    public DeterministicObjectIdentifier GetDeterministicObjectIdentifierFor(object instance)
    {
        return new DeterministicObjectIdentifier()
        {
            Type = instance?.GetType(),
            Hash = HashCalculator.CalculateHash(instance),
            KeyHash = KeyHashCalculator.CalculateKeyHash(instance)
        };
    }
}