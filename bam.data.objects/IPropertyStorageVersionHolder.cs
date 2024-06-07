namespace Bam.Data.Objects;

public interface IPropertyStorageVersionHolder: IPropertyStorageHolder
{
    int Version { get; }
}