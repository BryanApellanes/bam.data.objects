using System.Reflection;
using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
{
    IRootStorageHolder GetRootStorageHolder();
    ITypeStorageHolder GetTypeStorageHolder(Type type);

    IPropertyHolder GetPropertyStorageHolder(IProperty property);

    IStorage GetStorage(IStorageHolder storageIdentifier);

    IStorage GetRawStorage();

    bool IsSlotWritten(IStorageSlot slot);
    IPropertyStorageVersionSlot GetPropertySlot(IProperty property);
    IPropertyStorageVersionSlot GetPropertyVersionSlot(IProperty property, int version);
    int GetLatestVersionNumber(IProperty property);
    int GetNextVersionNumber(IProperty property);
    bool IsEqualToLatestVersion(IProperty property);
    bool VersionExists(IProperty property, int version = 1);
    IEnumerable<IPropertyVersion> GetVersionsAsync(IProperty property);
}
