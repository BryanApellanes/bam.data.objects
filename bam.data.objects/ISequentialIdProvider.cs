namespace Bam.Data.Objects;

public interface ISequentialIdProvider
{
    ulong GetNextSequentialULong();
}