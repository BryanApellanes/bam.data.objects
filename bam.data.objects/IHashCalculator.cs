namespace Bam.Data.Dynamic.Objects;

public interface IHashCalculator
{
    string CalculateHash(object instance);
}