using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriteResult : IObjectDataWriteResult
{
    public ObjectDataWriteResult(IObjectData data)
    {
        this.PropertyWriteResults = new Dictionary<string, IPropertyWriteResult>();
        this.ObjectData = data;
        this.Success = true;
    }
    
    public bool Success { get; set; }
    public string Message { get; set; }
    
    public IObjectData ObjectData { get; }
    public IObjectKey ObjectKey { get; set; }
    public IStorageSlot KeySlot { get; set; }
    public IDictionary<string, IPropertyWriteResult> PropertyWriteResults { get; init; }

    internal void AddPropertyWriteResult(IPropertyWriteResult propertyWriteResult)
    {
        PropertyWriteResults.Add(propertyWriteResult.Property?.PropertyName, propertyWriteResult);
    }
}