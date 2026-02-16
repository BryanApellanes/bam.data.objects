using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Represents the result of reading a property value.
/// </summary>
public interface IPropertyReadResult
{
    /// <summary>
    /// Gets the type that declares the property.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the property metadata that was read.
    /// </summary>
    PropertyInfo Property { get; }

    /// <summary>
    /// Gets the value that was read from the property.
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Gets a value indicating whether the read operation succeeded.
    /// </summary>
    bool Success { get; }
}