namespace Bam.Data.Objects;

public class Version : IVersion
{
    public Version(int number = 1, string description = null)
    {
        this.Number = number;
        this.Description = description;
    }

    public Version(byte[]? Value, int number = 1, string description = null)
    {
        this.Value = Value;
        this.Number = number;
        this.Description = description;
    }
    
    public int Number { get; }
    public string Description { get; }
    public byte[]? Value { get; }
}