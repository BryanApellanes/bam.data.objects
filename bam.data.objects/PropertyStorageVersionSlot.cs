using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public class PropertyStorageVersionSlot : PropertyStorageSlot, IPropertyStorageVersionSlot
{
    public PropertyStorageVersionSlot(IPropertyStorageHolder storageHolder, int version) : base(storageHolder)
    {
        this.Version = version;
        this.PropertyStorageVersionHolder = new PropertyStorageVersionHolder(storageHolder.FullName, version);
        this.StorageHolder = this.PropertyStorageVersionHolder;
    }

    public IPropertyStorageVersionHolder PropertyStorageVersionHolder { get; }
    public int Version { get; }
    public string? FullName => Path.Combine(PropertyStorageVersionHolder.FullName, Name);
    public override string Name => "dat";

    public IProperty Load(IObjectStorageManager storageManager)
    {
        throw new NotImplementedException();
    }
    
    public override IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property)
    {
        return storageManager.WriteProperty(property);
    }
}