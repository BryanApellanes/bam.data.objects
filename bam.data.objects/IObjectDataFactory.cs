using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Factory for creating <see cref="IObjectData"/> wrappers, computing keys and identifiers, and reconstructing properties from raw data.
/// </summary>
public interface IObjectDataFactory
{
    /// <summary>
    /// Gets or sets a value indicating whether sequential IDs should be assigned to created objects.
    /// </summary>
    bool SetSequentialIds { get; set; }

    /// <summary>
    /// Gets the locator factory used to compute keys and identifiers.
    /// </summary>
    IObjectDataLocatorFactory ObjectDataLocatorFactory { get; }

    /// <summary>
    /// Gets the encoder/decoder used for serializing and deserializing object data.
    /// </summary>
    IObjectEncoderDecoder ObjectEncoderDecoder { get; }

    /// <summary>
    /// Wraps a plain object in an <see cref="IObjectData"/> instance, reusing an existing wrapper if the object already implements <see cref="IObjectData"/>.
    /// </summary>
    /// <param name="data">The object to wrap.</param>
    /// <returns>An <see cref="IObjectData"/> wrapper for the object.</returns>
    IObjectData GetObjectData(object data);

    /// <summary>
    /// Computes the composite key for the specified object data.
    /// </summary>
    /// <param name="data">The object data to compute the key for.</param>
    /// <returns>The computed object key.</returns>
    IObjectDataKey GetObjectKey(IObjectData data);

    /// <summary>
    /// Computes the content-based identifier for the specified object data.
    /// </summary>
    /// <param name="data">The object data to compute the identifier for.</param>
    /// <returns>The computed object identifier.</returns>
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);

    /// <summary>
    /// Reconstructs a property from raw data using the specified parent object data and property descriptor.
    /// </summary>
    /// <param name="parent">The parent object data that owns the property.</param>
    /// <param name="propertyDescriptor">The descriptor identifying the property.</param>
    /// <param name="rawData">The raw data to decode into a property value.</param>
    /// <returns>The reconstructed property.</returns>
    IProperty PropertyFromRawData(IObjectData parent, IPropertyDescriptor propertyDescriptor, IRawData rawData);
}