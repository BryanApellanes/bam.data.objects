namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataSearchIndexResult"/>.
/// </summary>
public class ObjectDataSearchIndexResult : IObjectDataSearchIndexResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public int PropertiesIndexed { get; set; }
}
