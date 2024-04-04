namespace Bam.Data.Objects;

public interface IObjectKey
{
    TypeDescriptor Type { get; set; }
    ulong[] CompositeKeyPropertyIdentifiers { get; set; }
}