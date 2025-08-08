namespace Bam.Data.Objects;

/// <summary>
/// Provides unique identifiers for objects with properties adorned with the
/// <see cref="Bam.Data.Repositories.CompositeKeyAttribute" />
/// </summary>
public interface ICompositeKeyCalculator
{
    ulong CalculateULongKey(object instance);
    ulong CalculateULongKey(IObjectData objectData);
    string CalculateHashHexKey(object instance);
    string CalculateHashHexKey(IObjectData objectData);
}