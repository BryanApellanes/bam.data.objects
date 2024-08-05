using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataFactory : IObjectDataFactory
{
    public ObjectDataFactory(IObjectDataLocatorFactory objectDataLocatorFactory, IObjectEncoderDecoder encoderDecoder)
    {
        this.ObjectDataLocatorFactory = objectDataLocatorFactory;
        this.ObjectEncoderDecoder = encoderDecoder;
    }
    
    public IObjectDataLocatorFactory ObjectDataLocatorFactory { get; init; }
    public  IObjectEncoderDecoder ObjectEncoderDecoder { get; init; }

    public IObjectData Wrap(object data)
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

    public IObjectDataKey GetObjectKey(IObjectData data)
    {
        return ObjectDataLocatorFactory.GetObjectKey(data);
    }

    public IObjectDataIdentifier GetObjectIdentifier(IObjectData data)
    {
        return ObjectDataLocatorFactory.GetObjectIdentifier(data);
    }

    public IProperty PropertyFromRawData(IObjectData parent, IPropertyDescriptor propertyDescriptor, IRawData rawData)
    {
        parent.ObjectDataLocatorFactory = parent.ObjectDataLocatorFactory ?? ObjectDataLocatorFactory;
        return Property.FromRawData(parent, this.ObjectEncoderDecoder, propertyDescriptor, rawData);
    }
}