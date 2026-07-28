namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of an object data archive operation.
/// </summary>
public interface IObjectDataArchiveResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the archive operation succeeded.
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// Gets or sets the file-system path the object's storage was moved to, populated on
    /// success.  The archived bytes remain readable there — archiving removes an object from
    /// live storage and indexes without destroying it.
    /// </summary>
    string ArchivePath { get; set; }
}