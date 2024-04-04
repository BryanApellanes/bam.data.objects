using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;



public class ObjectIdentifierFactory : IObjectIdentifierFactory
{
    public ObjectIdentifierFactory(IKeyHashCalculator keyHashCalculator, IHashCalculator hashCalculator)
    {
        this.KeyHashCalculator = keyHashCalculator;
        this.HashCalculator = hashCalculator;
    }

    private IKeyHashCalculator KeyHashCalculator { get; set; }
    private IHashCalculator HashCalculator { get; set; }
    
    public IObjectIdentifier GetObjectIdentifierFor(object instance)
    {
        ObjectData data = new ObjectData(instance);
        // TODO: create ObjectKey used to find object identifier
        // object identifier identifies all properties
        // object key identifies indexed/searchable key properties
        return new ObjectIdentifier()
        {
            Type = data.TypeDescriptor,
            PropertyIdentifiers = data.Properties.Select(op => op.ToRawData().HashId).ToArray()
        };
    }
}