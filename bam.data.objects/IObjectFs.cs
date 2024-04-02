using System.Reflection;

namespace Bam.Storage;

public interface IObjectFs
{
    DirectoryInfo GetRootDirectory();
    DirectoryInfo GetTypeDirectory(Type type);
    DirectoryInfo GetPropertyDirectory(PropertyInfo property);
    DirectoryInfo GetPropertyDirectory(Type type, PropertyInfo property);
    DirectoryInfo GetKeysDirectory(PropertyInfo property);
    DirectoryInfo GetKeysDirectory(Type type, PropertyInfo property);
    DirectoryInfo GetHashesDirectory(PropertyInfo property);
    DirectoryInfo GetHashesDirectory(Type type, PropertyInfo property);
}