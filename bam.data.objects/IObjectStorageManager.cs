using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
{
    IRootStorageContainer GetRootStorageContainer();
    IStorageContainer GetTypeStorageContainer(Type type);
    IObjectPropertyStorageContainer GetPropertyStorageContainer(IObjectProperty property);
    IStorageContainer GetKeyStorageContainer(IObjectKey objectKey);
    IStorageContainer GetHashStorageIdentifier(IObjectIdentifier objectIdentifier);

    IStorage GetStorage(IStorageContainer storageIdentifier);
}
