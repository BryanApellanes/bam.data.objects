namespace Bam.Data.Objects;

/// <summary>
/// Provides sequential ulong identifiers, typically persisted to ensure uniqueness across process restarts.
/// </summary>
public interface ISequentialIdProvider
{
    /// <summary>
    /// Gets the next sequential ulong identifier, incrementing the internal counter.
    /// </summary>
    /// <returns>The next sequential identifier.</returns>
    ulong GetNextSequentialULong();
}