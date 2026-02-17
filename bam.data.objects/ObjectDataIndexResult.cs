namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataIndexResult"/>.
/// </summary>
public class ObjectDataIndexResult : IObjectDataIndexResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public ulong Id { get; set; }

    /// <inheritdoc />
    public IObjectDataKey ObjectDataKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    public string Message { get; set; } = null!;
}
