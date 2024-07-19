namespace Bam.Data.Objects;

public interface IPropertyVersionPointer
{
    IObjectDataKey ObjectDataKey { get; }
    IProperty Property { get; }
}