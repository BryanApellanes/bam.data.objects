namespace Bam.Data.Objects;

public interface IObjectDataFactory
{
    IObjectData Wrap(object data);
    IObjectDataKey GetObjectKey(IObjectData data);
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);
}