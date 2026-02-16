namespace Bam.Data.Objects;

/// <summary>
/// Represents the possible outcomes of a property write operation.
/// </summary>
public enum PropertyWriteResults
{
    /// <summary>
    /// The write result has not been determined.
    /// </summary>
    Undetermined,

    /// <summary>
    /// The property was written successfully.
    /// </summary>
    Success,

    /// <summary>
    /// The property write operation failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The property value was already saved and unchanged from the latest revision.
    /// </summary>
    AlreadySaved
}