using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public interface IHashCalculator
{
    ulong CalculateULongHash(object data);
    ulong CalculateULongHash(IObjectData data);
    string CalculateHashHex(object data);
    string CalculateHashHex(IObjectData data);
}