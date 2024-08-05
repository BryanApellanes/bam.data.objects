using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataFactory
{
    IObjectDataLocatorFactory ObjectDataLocatorFactory { get; }
    IObjectEncoderDecoder ObjectEncoderDecoder { get; }
    
    IObjectData Wrap(object data);
    IObjectDataKey GetObjectKey(IObjectData data);
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);
    IProperty PropertyFromRawData(IObjectData parent, IPropertyDescriptor propertyDescriptor, IRawData rawData);
}