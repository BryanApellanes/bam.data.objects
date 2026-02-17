namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataDeleteResult"/>.
/// </summary>
public class ObjectDataDeleteResult : IObjectDataDeleteResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; } = null!;
}
