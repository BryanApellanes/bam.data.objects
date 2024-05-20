using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public class PropertyStorageVersionSlot : PropertyStorageSlot, IPropertyStorageVersionSlot
{
    public PropertyStorageVersionSlot(PropertyStorageHolder storageHolder, int version) : base(storageHolder)
    {
        this.Version = version;
    }

    public int Version { get; }
    public IProperty Load(IObjectStorageManager storageManager)
    {
        throw new NotImplementedException();
    }

    public override IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property)
    {
        throw new NotImplementedException();
    }
}