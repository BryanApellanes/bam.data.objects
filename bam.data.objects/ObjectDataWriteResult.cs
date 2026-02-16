using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataWriteResult"/> containing per-property write results and overall status.
/// </summary>
public class ObjectDataWriteResult : IObjectDataWriteResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataWriteResult"/> class for the specified object data.
    /// </summary>
    /// <param name="data">The object data that was written.</param>
    public ObjectDataWriteResult(IObjectData data)
    {
        this.PropertyWriteResults = new Dictionary<string, IPropertyWriteResult>();
        this.ObjectData = data;
        this.Success = true;
    }
    
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <summary>
    /// Gets the type descriptor from the written object data.
    /// </summary>
    public TypeDescriptor TypeDescriptor
    {
        get => ObjectData?.TypeDescriptor;
    }

    /// <inheritdoc />
    public IObjectData ObjectData { get; }

    /// <inheritdoc />
    public IObjectDataKey ObjectDataKey => this.ObjectData.GetObjectKey();

    /// <inheritdoc />
    public IStorageSlot KeySlot { get; set; }

    /// <inheritdoc />
    public IDictionary<string, IPropertyWriteResult> PropertyWriteResults { get; init; }

    internal void AddPropertyWriteResult(IPropertyWriteResult propertyWriteResult)
    {
        PropertyWriteResults.Add(propertyWriteResult.Property?.PropertyName, propertyWriteResult);
    }
}