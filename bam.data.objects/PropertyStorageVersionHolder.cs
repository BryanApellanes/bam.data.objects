namespace Bam.Data.Objects;

public class PropertyStorageVersionHolder : PropertyStorageHolder, IPropertyStorageVersionHolder
{
    public PropertyStorageVersionHolder(string path, int version) : base(path)
    {
        this.Version = version;
    }

    public PropertyStorageVersionHolder(DirectoryInfo directory, int version) : base(directory)
    {
        this.Version = version;
    }

    public int Version { get; }

    public override string? FullName => Path.Combine(base.FullName, Version.ToString());
}