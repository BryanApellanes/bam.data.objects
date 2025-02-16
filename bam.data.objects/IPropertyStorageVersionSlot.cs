namespace Bam.Data.Objects;

public interface IPropertyStorageVersionSlot : IPropertyStorageSlot
{
    IPropertyStorageVersionHolder PropertyStorageVersionHolder { get; }
    int Version { get; }
    string MetaData { get; set; }
}