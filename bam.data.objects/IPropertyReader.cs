using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Defines operations for reading individual property values from a type.
/// </summary>
public interface IPropertyReader
{
    /// <summary>
    /// Reads the value of the specified property from the specified type.
    /// </summary>
    /// <param name="type">The type that declares the property.</param>
    /// <param name="property">The property metadata to read.</param>
    /// <returns>The result of the read operation, containing the property value.</returns>
    IPropertyReadResult ReadProperty(Type type, PropertyInfo property);

    /// <summary>
    /// Reads the value of the specified property from the specified type, returning a strongly-typed result.
    /// </summary>
    /// <typeparam name="TValue">The expected type of the property value.</typeparam>
    /// <param name="type">The type that declares the property.</param>
    /// <param name="property">The property metadata to read.</param>
    /// <returns>The strongly-typed result of the read operation.</returns>
    IPropertyReadResult<TValue> ReadProperty<TValue>(Type type, PropertyInfo property);

}