using System.Reflection;
using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
{
    IRootStorageHolder GetRootStorageHolder();
    ITypeStorageHolder GetTypeStorageHolder(Type type);

    IPropertyStorageHolder GetPropertyStorageHolder(IProperty property);

    IStorage GetStorage(IStorageHolder storageIdentifier);

    IStorage GetRawStorage();
}
