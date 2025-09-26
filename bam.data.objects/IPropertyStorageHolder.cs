using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageHolder : IStorageHolder
{
    string PropertyName { get; }
    ITypeStorageHolder TypeStorageHolder { get; }
    IPropertyStorageRevisionSlot GetPropertyVersionSlot(IObjectDataStorageManager objectDataStorageManager, IProperty property, int version);
    IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);

    IEnumerable<IPropertyStorageRevisionSlot> GetVersions(IObjectDataStorageManager dataStorageManager, IProperty property);
}