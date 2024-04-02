namespace Bam.Data.Dynamic.Objects;

public interface IDeterministicObjectIdentifierFactory
{
    DeterministicObjectIdentifier GetDeterministicObjectIdentifierFor(object instance);
}