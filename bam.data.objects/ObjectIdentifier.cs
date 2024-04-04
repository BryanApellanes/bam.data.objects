namespace Bam.Data.Objects;



public class ObjectIdentifier : IObjectIdentifier
{
    public TypeDescriptor Type { get; set; }
    public ulong[] PropertyIdentifiers { get; set; }
}