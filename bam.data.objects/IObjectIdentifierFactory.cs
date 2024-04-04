namespace Bam.Data.Objects;

public interface IObjectIdentifierFactory
{
    IObjectIdentifier GetObjectIdentifierFor(object instance);
}