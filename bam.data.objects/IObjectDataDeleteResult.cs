namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of a delete operation on object data.
/// </summary>
public interface IObjectDataDeleteResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the delete operation succeeded.
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; set; }
}