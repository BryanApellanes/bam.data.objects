namespace Bam.Data.Objects;

public interface IPropertyDescriptor
{
    IObjectKey ObjectKey { get; set; }
    string PropertyName { get; set; }
}