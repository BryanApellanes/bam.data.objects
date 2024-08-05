namespace Bam.Data.Objects;

public interface IObjectDataKey
{
    TypeDescriptor TypeDescriptor { get; }
    string? Key { get; }
    string GetPath(IObjectDataStorageManager? objectDataStorageManager);

    IPropertyDescriptor Property(string propertyName);
}