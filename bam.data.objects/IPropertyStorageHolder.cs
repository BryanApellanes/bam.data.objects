using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageHolder : IStorageHolder
{
    string PropertyName { get; }
    ITypeStorageHolder TypeStorageHolder { get; }
    IPropertyStorageVersionSlot GetPropertyVersionSlot(IObjectDataStorageManager objectDataStorageManager, IProperty property, int version);
    IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);

    IEnumerable<IPropertyStorageVersionSlot> GetVersions(IObjectDataStorageManager dataStorageManager, IProperty property);
}