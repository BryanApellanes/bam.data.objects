using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace bam.data.dynamic.Objects;

/// <summary>
/// Strongly-typed implementation of <see cref="IPropertyReadResult{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the property value.</typeparam>
public class PropertyReadResult<TValue> : IPropertyReadResult<TValue>
{
    /// <inheritdoc />
    public Type Type { get; } = null!;

    /// <inheritdoc />
    public PropertyInfo Property { get; } = null!;

    /// <inheritdoc />
    public TValue Value { get; } = default!;

    /// <inheritdoc />
    object IPropertyReadResult.Value => Value!;

    /// <inheritdoc />
    public bool Success { get; }
}