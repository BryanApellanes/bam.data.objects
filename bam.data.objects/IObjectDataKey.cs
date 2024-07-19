namespace Bam.Data.Objects;

public interface IObjectDataKey: IObjectDataIdentifier
{
    string Key { get; }
    string GetPath();

    IPropertyDescriptor Property(string propertyName);
}