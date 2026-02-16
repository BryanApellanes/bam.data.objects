using Bam.Data.Objects;

namespace Bam.Storage;

/// <summary>
/// Represents the result of saving object data to storage.
/// </summary>
public interface IObjectDataStorageSaveResult
{
    /// <summary>
    /// Gets or sets the reader that can be used to read back the saved object data.
    /// </summary>
    IObjectDataReader ObjectDataReader { get; set; }
}