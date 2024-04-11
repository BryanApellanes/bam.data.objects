namespace Bam.Data.Objects;

public interface IObjectDataFactory
{
    IObjectData Wrap(object data);
    IObjectKey GetObjectKey(IObjectData data);
    IObjectIdentifier GetObjectIdentifier(IObjectData data);
}