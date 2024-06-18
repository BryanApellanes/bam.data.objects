namespace Bam.Data.Objects;

public interface IPropertyDescriptor
{
    Type Type { get; set; }
    string PropertyName { get; set; }
}