using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Repositories;

/// <summary>
/// Default implementation of <see cref="IPropertyReadResult"/>.
/// </summary>
public class PropertyReadResult : IPropertyReadResult
{
    /// <inheritdoc />
    public Type Type { get; } = null!;

    /// <inheritdoc />
    public PropertyInfo Property { get; } = null!;

    /// <inheritdoc />
    public object Value { get; } = null!;

    /// <inheritdoc />
    public bool Success { get; }
}