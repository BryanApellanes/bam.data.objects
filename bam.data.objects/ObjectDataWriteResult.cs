using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public class ObjectDataWriteResult : IObjectDataWriteResult
{
    public ObjectDataWriteResult(IObjectData data)
    {
        this.PropertyWriteResults = new Dictionary<string, IObjectPropertyWriteResult>();
        this.Data = data;
    }
    
    public bool Success { get; set; }
    public string Message { get; set; }
    
    public IObjectData Data { get; }
    public IObjectKey ObjectKey { get; set; }
    public IDictionary<string, IObjectPropertyWriteResult> PropertyWriteResults { get; init; }

    public void AddPropertyWriteResult(IObjectPropertyWriteResult propertyWriteResult)
    {
        PropertyWriteResults.Add(propertyWriteResult.ObjectProperty?.PropertyName, propertyWriteResult);
    }
}