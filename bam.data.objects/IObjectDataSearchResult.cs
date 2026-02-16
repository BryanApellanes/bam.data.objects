namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of a search operation, containing the matching objects and count.
/// </summary>
public interface IObjectDataSearchResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the search operation succeeded.
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// Gets the collection of matching object data results.
    /// </summary>
    IEnumerable<IObjectData> Results { get; }

    /// <summary>
    /// Gets the total number of matching results.
    /// </summary>
    int TotalCount { get; }
}
