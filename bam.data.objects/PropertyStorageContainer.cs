using Bam.Storage;

namespace Bam.Data.Objects;

public class PropertyStorageContainer : DirectoryStorageContainer
{
    public PropertyStorageContainer(string path) : base(path)
    {
    }

    public PropertyStorageContainer(DirectoryInfo directory) : base(directory)
    {
    }

    public override string? FullName => Path.Combine(base.FullName, GetNextVersion().ToString());
    
    private int GetNextVersion()
    {
        int number = 1;
        while (System.IO.Directory.Exists(Path.Combine(base.FullName, number.ToString())))
        {
            number++;
        }
        return number;
    }
}