using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public class PropertyStorageRevisionSlot : PropertyStorageSlot, IPropertyStorageRevisionSlot
{
    public PropertyStorageRevisionSlot(IPropertyStorageHolder storageHolder, int version) : base(storageHolder)
    {
        this.Revision = version;
        this.PropertyStorageRevisionHolder = new PropertyStorageRevisionHolder(storageHolder.FullName, version);
        this.StorageHolder = this.PropertyStorageRevisionHolder;
    }

    public IPropertyStorageRevisionHolder PropertyStorageRevisionHolder { get; }
    public int Revision { get; }
    public string? FullName => Path.Combine(PropertyStorageRevisionHolder.FullName, Name);
    public override string Name => "dat";

    public string MetaData
    {
        get
        {
            if (File.Exists(this.MetaDataFile))
            {
                return File.ReadAllText(this.MetaDataFile);
            }

            return string.Empty;
        }
        set
        {
            File.WriteAllText(this.MetaDataFile, value);
        }
    }
    
    public override IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        return dataStorageManager.WriteProperty(property);
    }

    private string MetaDataFile => Path.Combine(PropertyStorageRevisionHolder.FullName, "meta");
}