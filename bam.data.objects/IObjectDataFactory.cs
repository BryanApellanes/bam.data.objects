using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataFactory
{
    bool SetSequentialIds { get; set; }
    
    IObjectDataLocatorFactory ObjectDataLocatorFactory { get; }
    IObjectEncoderDecoder ObjectEncoderDecoder { get; }
    
    IObjectData GetObjectData(object data);
    IObjectDataKey GetObjectKey(IObjectData data);
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);
    IProperty PropertyFromRawData(IObjectData parent, IPropertyDescriptor propertyDescriptor, IRawData rawData);
}