namespace Bam.Data.Objects;

public class ObjectDataSearchResult : IObjectDataSearchResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public IEnumerable<IObjectData> Results { get; set; }
    public int TotalCount { get; set; }
}
