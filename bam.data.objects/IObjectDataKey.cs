namespace Bam.Data.Objects;

public interface IObjectDataKey: IObjectDataIdentifier
{
    string Key { get; }
    string GetPath(IObjectDataStorageManager? objectDataStorageManager);

    IPropertyDescriptor Property(string propertyName);
}