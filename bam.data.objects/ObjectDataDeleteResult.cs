namespace Bam.Data.Objects;

public class ObjectDataDeleteResult : IObjectDataDeleteResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
