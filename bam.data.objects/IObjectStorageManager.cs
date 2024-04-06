using System.Reflection;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
{
    IStorageContainer GetRootStorageContainer();
    IStorageContainer GetTypeStorageContainer(Type type);
    IStorageContainer GetPropertyStorageContainer(PropertyInfo property);
    IStorageContainer GetPropertyStorageContainer(Type type, PropertyInfo property);
    IStorageContainer GetKeyStorageContainer(IObjectKey objectKey);
    IStorageContainer GetHashStorageIdentifier(IObjectIdentifier objectIdentifier);

    IStorage GetStorage(IStorageContainer storageIdentifier);
}
