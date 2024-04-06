using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IHashCalculator
{
    ulong CalculateHash(object data);
    ulong CalculateHash(IObjectData data);
    
}