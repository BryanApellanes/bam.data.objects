using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriteResult : IObjectDataWriteResult
{
    public ObjectDataWriteResult(IObjectData data)
    {
        this.PropertyWriteResults = new Dictionary<string, IObjectPropertyWriteResult>();
        this.ObjectData = data;
        this.Success = true;
    }
    
    public bool Success { get; set; }
    public string Message { get; set; }
    
    public IObjectData ObjectData { get; }
    public IObjectKey ObjectKey { get; set; }
    public IStorageSlot KeySlot { get; set; }
    public IDictionary<string, IObjectPropertyWriteResult> PropertyWriteResults { get; init; }

    internal void AddPropertyWriteResult(IObjectPropertyWriteResult propertyWriteResult)
    {
        PropertyWriteResults.Add(propertyWriteResult.ObjectProperty?.PropertyName, propertyWriteResult);
    }
}