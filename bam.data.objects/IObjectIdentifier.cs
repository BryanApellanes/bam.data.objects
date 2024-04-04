namespace Bam.Data.Objects;

public interface IObjectIdentifier
{
    TypeDescriptor Type { get; set; }
    ulong[] PropertyIdentifiers { get; set; }
}