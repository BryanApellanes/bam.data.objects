using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
{
    IRootStorageHolder GetRootStorageContainer();
    IStorageHolder GetTypeStorageContainer(Type type);

    IObjectPropertyStorageHolder GetPropertyStorageContainer(IObjectProperty property);

    IStorage GetKeyStorage(IObjectKey objectKey);

    IStorageHolder GetKeyStorageContainer(IObjectKey objectKey);

    IStorage GetStorage(IStorageHolder storageIdentifier);

    IStorage GetRawStorage();
}
