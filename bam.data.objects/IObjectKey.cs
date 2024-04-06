namespace Bam.Data.Objects;

public interface IObjectKey: IObjectIdentifier
{
    ulong Key { get; }
}