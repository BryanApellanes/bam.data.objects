using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
{
    IRootStorageContainer GetRootStorageContainer();
    IStorageContainer GetTypeStorageContainer(Type type);

    IObjectPropertyStorageContainer GetPropertyStorageContainer(IObjectProperty property);

    IStorage GetKeyStorage(IObjectKey objectKey);

    IStorageContainer GetKeyStorageContainer(IObjectKey objectKey);
    /*IStorage GetHashStorage(IObjectIdentifier objectIdentifier);
    IStorageContainer GetHashStorageContainer(IObjectIdentifier objectIdentifier);*/

    IStorage GetStorage(IStorageContainer storageIdentifier);

    IStorage GetRawStorage();
}
