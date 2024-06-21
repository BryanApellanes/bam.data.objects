namespace Bam.Data.Objects;

public interface IObjectKey: IObjectIdentifier
{
    string Key { get; }
    string GetPath();

    IPropertyDescriptor Property(string propertyName);
}