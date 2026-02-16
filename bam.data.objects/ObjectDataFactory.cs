using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataFactory"/> that creates object data wrappers, computes keys and identifiers, and reconstructs properties from raw data.
/// </summary>
public class ObjectDataFactory : IObjectDataFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataFactory"/> class.
    /// </summary>
    /// <param name="objectDataLocatorFactory">The locator factory used to compute keys and identifiers.</param>
    /// <param name="encoderDecoder">The encoder/decoder used for serializing and deserializing object data.</param>
    public ObjectDataFactory(IObjectDataLocatorFactory objectDataLocatorFactory, IObjectEncoderDecoder encoderDecoder)
    {
        this.ObjectDataLocatorFactory = objectDataLocatorFactory;
        this.ObjectEncoderDecoder = encoderDecoder;
    }

    /// <inheritdoc />
    public bool SetSequentialIds { get; set; }

    /// <inheritdoc />
    public IObjectDataLocatorFactory ObjectDataLocatorFactory { get; init; }

    /// <inheritdoc />
    public  IObjectEncoderDecoder ObjectEncoderDecoder { get; init; }

    /// <inheritdoc />
    public IObjectData GetObjectData(object data)
    {
        if (data is ObjectData objectData)
        {
            objectData.ObjectDataLocatorFactory ??= this.ObjectDataLocatorFactory;
            return objectData;
        }

        if (data is IObjectData iObjectData)
        {
            iObjectData.ObjectDataLocatorFactory ??= this.ObjectDataLocatorFactory;
            return iObjectData;
        }

        return new ObjectData(data, ObjectEncoderDecoder){ObjectDataLocatorFactory = this.ObjectDataLocatorFactory};
    }

    /// <inheritdoc />
    public IObjectDataKey GetObjectKey(IObjectData data)
    {
        return ObjectDataLocatorFactory.GetObjectKey(data);
    }

    /// <inheritdoc />
    public IObjectDataIdentifier GetObjectIdentifier(IObjectData data)
    {
        return ObjectDataLocatorFactory.GetObjectIdentifier(data);
    }

    /// <inheritdoc />
    public IProperty PropertyFromRawData(IObjectData parent, IPropertyDescriptor propertyDescriptor, IRawData rawData)
    {
        parent.ObjectDataLocatorFactory = parent.ObjectDataLocatorFactory ?? ObjectDataLocatorFactory;
        return Property.FromRawData(parent, this.ObjectEncoderDecoder, propertyDescriptor, rawData);
    }
}