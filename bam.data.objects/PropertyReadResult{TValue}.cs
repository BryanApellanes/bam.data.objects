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
    public Type Type { get; }

    /// <inheritdoc />
    public PropertyInfo Property { get; }

    /// <inheritdoc />
    public TValue Value { get; }

    /// <inheritdoc />
    object IPropertyReadResult.Value => Value;

    /// <inheritdoc />
    public bool Success { get; }
}