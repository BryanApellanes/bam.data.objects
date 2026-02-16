using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IPropertyStorageRevisionSlot"/> that represents a specific versioned storage slot for a property, stored as a "dat" file.
/// </summary>
public class PropertyStorageRevisionSlot : PropertyStorageSlot, IPropertyStorageRevisionSlot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStorageRevisionSlot"/> class.
    /// </summary>
    /// <param name="storageHolder">The property storage holder that contains this slot.</param>
    /// <param name="version">The revision version number.</param>
    public PropertyStorageRevisionSlot(IPropertyStorageHolder storageHolder, int version) : base(storageHolder)
    {
        this.Revision = version;
        this.PropertyStorageRevisionHolder = new PropertyStorageRevisionHolder(storageHolder.FullName, version);
        this.StorageHolder = this.PropertyStorageRevisionHolder;
    }

    /// <inheritdoc />
    public IPropertyStorageRevisionHolder PropertyStorageRevisionHolder { get; }

    /// <inheritdoc />
    public int Revision { get; }

    /// <summary>
    /// Gets the full file path for this revision slot.
    /// </summary>
    public string? FullName => Path.Combine(PropertyStorageRevisionHolder.FullName, Name);

    /// <summary>
    /// Gets the file name for this slot, always "dat".
    /// </summary>
    public override string Name => "dat";

    /// <inheritdoc />
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
    
    /// <inheritdoc />
    public override IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        return dataStorageManager.WriteProperty(property);
    }

    private string MetaDataFile => Path.Combine(PropertyStorageRevisionHolder.FullName, "meta");
}