namespace Bam.Data.Objects;

public interface IPropertyStorageRevisionHolder: IPropertyStorageHolder
{
    int Version { get; }
}