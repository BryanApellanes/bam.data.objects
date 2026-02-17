using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;



/// <summary>
/// Default implementation of <see cref="IPropertyWriteResult"/>, containing the results of writing a property to storage.
/// </summary>
public class PropertyWriteResult : IPropertyWriteResult
{
    /// <inheritdoc />
    public IObjectDataKey ObjectDataKey { get; set; } = null!;

    /// <inheritdoc />
    public IStorageSlot PointerStorageSlot { get; set; } = null!;

    /// <inheritdoc />
    public IStorageSlot ValueStorageSlot { get; set; } = null!;

    /// <inheritdoc />
    public IProperty Property { get; set; } = null!;

    /// <inheritdoc />
    public IRawData RawData { get; set; } = null!;

    /// <inheritdoc />
    public PropertyWriteResults Status { get; set; }

    /// <inheritdoc />
    public string Message { get; set; } = null!;

    /// <inheritdoc />
    public string RawDataHash { get; set; } = null!;

    /// <inheritdoc />
    public IPropertyDescriptor GetDescriptor()
    {
        return new PropertyDescriptor()
        {
            ObjectDataKey = this.ObjectDataKey,
            PropertyName = Property.PropertyName
        };
    }
}