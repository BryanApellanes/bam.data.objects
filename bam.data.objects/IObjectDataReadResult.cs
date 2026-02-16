namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of reading object data from storage.
/// </summary>
public interface IObjectDataReadResult
{
    /// <summary>
    /// Gets the object data that was read, or null if the read failed.
    /// </summary>
    IObjectData ObjectData { get; }

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the read operation succeeded.
    /// </summary>
    bool Success { get; set; }
}