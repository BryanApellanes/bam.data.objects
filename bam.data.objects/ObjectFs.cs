using System.Reflection;
using Bam.Net;
using Bam.Storage;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

public class ObjectFs : IObjectFs
{
    public ObjectFs(ObjectFsRootDirectory rootDirectory, IObjectHashCalculator objectHashCalculator)
    {
        this.RootDirectory = rootDirectory;
        this.ObjectHashCalculator = objectHashCalculator;
    }
    
    public ObjectFsRootDirectory RootDirectory { get; private set; }
    public IObjectHashCalculator ObjectHashCalculator { get; private set; }
    
    public DirectoryInfo GetRootDirectory()
    {
        return RootDirectory;
    }

    public DirectoryInfo GetTypeDirectory(Type type)
    {
        return new DirectoryInfo(Path.Combine(GetRootDirectory().FullName, GetRelativePathForType(type)));
    }

    public DirectoryInfo GetPropertyDirectory(PropertyInfo property)
    {
        return new DirectoryInfo(Path.Combine(GetTypeDirectory(property.DeclaringType).FullName, property.Name));
    }

    public DirectoryInfo GetPropertyDirectory(Type type, PropertyInfo property)
    {
        return new DirectoryInfo(Path.Combine(GetTypeDirectory(type).FullName, property.Name));
    }

    public DirectoryInfo GetKeysDirectory(PropertyInfo property)
    {
        return GetKeysDirectory(property.DeclaringType, property);
    }

    public DirectoryInfo GetKeysDirectory(Type type, PropertyInfo property)
    {
        DirectoryInfo directoryInfo = GetPropertyDirectory(type, property);
        return new DirectoryInfo(Path.Combine(directoryInfo.FullName, "keys"));
    }

    public DirectoryInfo GetHashesDirectory(PropertyInfo property)
    {
        return GetHashesDirectory(property.DeclaringType, property);
    }

    public DirectoryInfo GetHashesDirectory(Type type, PropertyInfo property)
    {
        DirectoryInfo directoryInfo = GetPropertyDirectory(type, property);
        return new DirectoryInfo(Path.Combine(directoryInfo.FullName, "hashes"));
    }

    private string GetRelativePathForType(Type type)
    {
        Args.ThrowIfNull(type, nameof(type));
        List<string> parts = new List<string>();
        parts.AddRange(type.FullName.DelimitSplit("."));
        return Path.Combine(parts.ToArray());
    }
}