namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataArchiveResult"/>.
/// </summary>
public class ObjectDataArchiveResult : IObjectDataArchiveResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; } = null!;

    /// <inheritdoc />
    public string ArchivePath { get; set; } = null!;
}
