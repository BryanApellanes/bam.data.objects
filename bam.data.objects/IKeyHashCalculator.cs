namespace Bam.Data.Dynamic.Objects;

public interface IKeyHashCalculator
{
    string CalculateKeyHash(object instance);
}