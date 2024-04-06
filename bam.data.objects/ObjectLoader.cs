namespace Bam.Data.Objects;



public class ObjectLoader : IObjectLoader
{
    public TypeDescriptor Type { get; set; }
    public ulong[] PropertyIdentifiers { get; set; }
}