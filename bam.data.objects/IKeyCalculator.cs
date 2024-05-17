namespace Bam.Data.Objects;

public interface IKeyCalculator
{
    ulong CalculateULongKey(object instance);
    ulong CalculateULongKey(IObjectData objectData);
    string CalculateHashHexKey(object instance);
    string CalculateHashHexKey(IObjectData objectData);
}