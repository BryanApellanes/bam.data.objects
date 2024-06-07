using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageHolder : IStorageHolder
{
    string PropertyName { get; }
    ITypeStorageHolder TypeStorageHolder { get; }
    IPropertyStorageVersionSlot GetPropertyVersionSlot(IObjectStorageManager objectStorageManager, IProperty property, int version);
    IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property);

    IEnumerable<IPropertyStorageVersionSlot> GetVersions(IObjectStorageManager storageManager, IProperty property);
}