namespace Bam.Data.Objects;

public class ObjectDataReadResult : IObjectDataReadResult
{
    public IObjectData ObjectData { get; internal set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}