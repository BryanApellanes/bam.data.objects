namespace Bam.Data.Objects;

public interface IObjectDataSearchResult
{
    bool Success { get; set; }
    string Message { get; set; }
    IEnumerable<IObjectData> Results { get; }
    int TotalCount { get; }
}
