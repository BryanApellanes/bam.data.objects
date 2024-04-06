namespace Bam.Data.Objects;

public interface IKeyHashCalculator
{
    ulong CalculateKeyHash(object instance);
}