using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyHolder : IStorageHolder
{
    string PropertyName { get; }
    ITypeStorageHolder TypeStorageHolder { get; }
    IPropertyStorageVersionSlot GetPropertyVersionSlot(IProperty property, int version);
    IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property);
}