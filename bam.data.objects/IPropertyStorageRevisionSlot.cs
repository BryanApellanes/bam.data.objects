namespace Bam.Data.Objects;

public interface IPropertyStorageRevisionSlot : IPropertyStorageSlot
{
    IPropertyStorageRevisionHolder PropertyStorageRevisionHolder { get; }
    int Revision { get; }
    string MetaData { get; set; }
}