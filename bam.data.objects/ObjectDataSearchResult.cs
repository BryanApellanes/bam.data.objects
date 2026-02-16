namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataSearchResult"/>.
/// </summary>
public class ObjectDataSearchResult : IObjectDataSearchResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public IEnumerable<IObjectData> Results { get; set; }

    /// <inheritdoc />
    public int TotalCount { get; set; }
}
