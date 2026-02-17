using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Represents the strongly-typed result of reading a property value.
/// </summary>
/// <typeparam name="TValue">The type of the property value.</typeparam>
public interface IPropertyReadResult<TValue> : IPropertyReadResult
{
    /// <summary>
    /// Gets the type that declares the property.
    /// </summary>
    new Type Type { get; }

    /// <summary>
    /// Gets the property metadata that was read.
    /// </summary>
    new PropertyInfo Property { get; }

    /// <summary>
    /// Gets the strongly-typed value that was read from the property.
    /// </summary>
    new TValue Value { get; }

    /// <summary>
    /// Gets a value indicating whether the read operation succeeded.
    /// </summary>
    new bool Success { get; }
}