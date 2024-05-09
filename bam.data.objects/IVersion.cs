namespace Bam.Data.Objects;

public interface IVersion
{
    int Number { get; }
    string Description { get; }
    byte[]? Value { get; }
}