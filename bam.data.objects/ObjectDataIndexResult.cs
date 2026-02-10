namespace Bam.Data.Objects;

public class ObjectDataIndexResult : IObjectDataIndexResult
{
    public bool Success { get; set; }
    public ulong Id { get; set; }
    public IObjectDataKey ObjectDataKey { get; set; }
    public string Message { get; set; }
}
