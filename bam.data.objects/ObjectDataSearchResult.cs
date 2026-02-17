namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataSearchResult"/>.
/// </summary>
public class ObjectDataSearchResult : IObjectDataSearchResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; } = null!;

    /// <inheritdoc />
    public IEnumerable<IObjectData> Results { get; set; } = null!;

    /// <inheritdoc />
    public int TotalCount { get; set; }
}
