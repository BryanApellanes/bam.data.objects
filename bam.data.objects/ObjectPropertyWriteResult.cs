namespace Bam.Data.Dynamic.Objects;



public class ObjectPropertyWriteResult : IObjectPropertyWriteResult
{
    public IObjectProperty ObjectProperty { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}