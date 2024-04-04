using System.Reflection;

namespace Bam.Storage;

public interface IObjectStorageManager
{
    IStorageIdentifier GetRootStorage();
    IStorageIdentifier GetTypeStorage(Type type);
    IStorageIdentifier GetPropertyStorage(PropertyInfo property);
    IStorageIdentifier GetPropertyStorage(Type type, PropertyInfo property);
    IStorageIdentifier GetKeyStorage(PropertyInfo property);
    IStorageIdentifier GetKeyStorage(Type type, PropertyInfo property);
    IStorageIdentifier GetHashStorage(PropertyInfo property);
    IStorageIdentifier GetHashStorage(Type type, PropertyInfo property);

    IStorage GetStorage(IStorageIdentifier storageIdentifier);
}