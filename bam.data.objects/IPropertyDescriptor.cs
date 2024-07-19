namespace Bam.Data.Objects;

public interface IPropertyDescriptor
{
    IObjectDataKey ObjectDataKey { get; set; }
    string PropertyName { get; set; }
    Type PropertyType { get; set; }
}