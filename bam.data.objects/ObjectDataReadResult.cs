namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataReadResult"/>.
/// </summary>
public class ObjectDataReadResult : IObjectDataReadResult
{
    /// <inheritdoc />
    public IObjectData ObjectData { get; internal set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public bool Success { get; set; }
}