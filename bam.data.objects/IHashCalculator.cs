namespace Bam.Data.Objects;

/// <summary>
/// A component used to calculate unique identifiers for objects.
/// </summary>
public interface IHashCalculator
{
    /// <summary>
    /// Calculates a hash as ulong for the current state of the data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>ulong</returns>
    ulong CalculateULongHash(object data);
    
    /// <summary>
    /// Calculates a hash as ulong for the current state of the data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>ulong</returns>
    ulong CalculateULongHash(IObjectData data);
    
    /// <summary>
    /// Calculates a hash as string for the current state of the data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>string</returns>
    string CalculateHashHex(object data);
    
    /// <summary>
    /// Calculates a hash as string for the current state of the data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>string</returns>
    string CalculateHashHex(IObjectData data);
}