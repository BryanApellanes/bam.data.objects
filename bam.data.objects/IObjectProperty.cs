namespace Bam.Data.Dynamic.Objects;

public interface IObjectProperty
{
    string TypeName { get; set; }
    string PropertyName { get; set; }
    string Value { get; set; }
}