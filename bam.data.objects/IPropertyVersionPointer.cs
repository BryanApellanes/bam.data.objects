namespace Bam.Data.Objects;

public interface IPropertyVersionPointer
{
    IObjectKey ObjectKey { get; }
    IProperty Property { get; }
}