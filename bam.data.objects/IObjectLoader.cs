namespace Bam.Data.Objects;

public interface IObjectLoader
{
    TypeDescriptor Type { get; set; }
    ulong[] PropertyIdentifiers { get; set; }
}