namespace Bam.Data.Objects;

public interface IKeyHashCalculator
{
    string CalculateKeyHash(object instance);
}