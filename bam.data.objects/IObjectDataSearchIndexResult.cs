namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of indexing an object's properties for search.
/// </summary>
public interface IObjectDataSearchIndexResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the indexing operation succeeded.
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// Gets or sets the number of properties that were indexed.
    /// </summary>
    int PropertiesIndexed { get; set; }
}
