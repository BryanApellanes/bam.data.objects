namespace Bam.Data.Objects;

/// <summary>
/// Provides unique identifiers for objects with properties adorned with the
/// <see cref="Bam.Data.Repositories.CompositeKeyAttribute" />
/// </summary>
public interface ICompositeKeyCalculator
{
    /// <summary>
    /// Calculates a composite key as a ulong from properties adorned with <see cref="Bam.Data.Repositories.CompositeKeyAttribute"/>.
    /// </summary>
    /// <param name="instance">The object instance to calculate the key for.</param>
    /// <returns>A ulong hash representing the composite key.</returns>
    ulong CalculateULongKey(object instance);

    /// <summary>
    /// Calculates a composite key as a ulong from properties adorned with <see cref="Bam.Data.Repositories.CompositeKeyAttribute"/>.
    /// </summary>
    /// <param name="objectData">The object data to calculate the key for.</param>
    /// <returns>A ulong hash representing the composite key.</returns>
    ulong CalculateULongKey(IObjectData objectData);

    /// <summary>
    /// Calculates a composite key as a hex-encoded hash string from properties adorned with <see cref="Bam.Data.Repositories.CompositeKeyAttribute"/>.
    /// </summary>
    /// <param name="instance">The object instance to calculate the key for.</param>
    /// <returns>A hex-encoded hash string representing the composite key.</returns>
    string CalculateHashHexKey(object instance);

    /// <summary>
    /// Calculates a composite key as a hex-encoded hash string from properties adorned with <see cref="Bam.Data.Repositories.CompositeKeyAttribute"/>.
    /// </summary>
    /// <param name="objectData">The object data to calculate the key for.</param>
    /// <returns>A hex-encoded hash string representing the composite key.</returns>
    string CalculateHashHexKey(IObjectData objectData);
}