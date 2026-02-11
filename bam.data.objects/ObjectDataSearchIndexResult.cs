namespace Bam.Data.Objects;

public class ObjectDataSearchIndexResult : IObjectDataSearchIndexResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int PropertiesIndexed { get; set; }
}
