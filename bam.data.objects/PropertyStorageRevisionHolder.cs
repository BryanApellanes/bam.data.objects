namespace Bam.Data.Objects;

public class PropertyStorageRevisionHolder : PropertyStorageHolder, IPropertyStorageRevisionHolder
{
    public PropertyStorageRevisionHolder(string path, int version) : base(path)
    {
        this.Version = version;
    }

    public PropertyStorageRevisionHolder(DirectoryInfo directory, int version) : base(directory)
    {
        this.Version = version;
    }

    public int Version { get; }

    public override string? FullName => Path.Combine(base.FullName, Version.ToString());
}