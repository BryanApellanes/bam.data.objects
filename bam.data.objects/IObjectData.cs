using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a wrapper around an arbitrary object that provides property-level access, encoding, and identity operations.
/// </summary>
public interface IObjectData : IJsonable
{
    /// <summary>
    /// Gets or sets the underlying data object.
    /// </summary>
    [JsonIgnore]
    [YamlIgnore]
    object Data { get; set; }

    /// <summary>
    /// Gets the type descriptor for the underlying data object.
    /// </summary>
    TypeDescriptor TypeDescriptor { get; }

    /// <summary>
    /// Gets or sets the factory used to create object data locators for key and identifier resolution.
    /// </summary>
    IObjectDataLocatorFactory ObjectDataLocatorFactory { get; set; }

    /// <summary>
    /// Gets the property with the specified name.
    /// </summary>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns>The property, or null if not found.</returns>
    IProperty? Property(string propertyName);

    /// <summary>
    /// Sets the value of the specified property on the underlying data object.
    /// </summary>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <returns>This object data instance for fluent chaining, or null if the property was not found.</returns>
    IObjectData? Property(string propertyName, object value);

    /// <summary>
    /// Gets the collection of properties extracted from the underlying data object.
    /// </summary>
    IEnumerable<IProperty> Properties { get; }

    /// <summary>
    /// Encodes the underlying data object into an <see cref="IObjectEncoding"/>.
    /// </summary>
    /// <returns>The encoded representation of the data.</returns>
    IObjectEncoding Encode();

    /// <summary>
    /// Computes and returns the object key, which uniquely identifies this object based on its composite key properties.
    /// </summary>
    /// <returns>The computed object key.</returns>
    IObjectDataKey GetObjectKey();

    /// <summary>
    /// Computes and returns the object identifier, which uniquely identifies this object based on its full content hash.
    /// </summary>
    /// <returns>The computed object identifier.</returns>
    IObjectDataIdentifier GetObjectIdentifier();
}