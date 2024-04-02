namespace Bam.Data.Dynamic.Objects;

public class ObjectFsRootDirectory
{
    public static implicit operator DirectoryInfo(ObjectFsRootDirectory objectFsRootDirectory)
    {
        return new DirectoryInfo(objectFsRootDirectory.Value);
    }

    public static implicit operator string(ObjectFsRootDirectory objectFsRootDirectory)
    {
        return objectFsRootDirectory.Value;
    }

    public ObjectFsRootDirectory()
    {
        this.Value = Environment.CurrentDirectory;
    }

    public ObjectFsRootDirectory(string path)
    {
        this.Value = path;
    }
    
    public string Value { get; set; }
}