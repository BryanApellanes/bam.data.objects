using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Repositories;

/// <summary>
/// Default implementation of <see cref="IPropertyReadResult"/>.
/// </summary>
public class PropertyReadResult : IPropertyReadResult
{
    /// <inheritdoc />
    public Type Type { get; }

    /// <inheritdoc />
    public PropertyInfo Property { get; }

    /// <inheritdoc />
    public object Value { get; }

    /// <inheritdoc />
    public bool Success { get; }
}