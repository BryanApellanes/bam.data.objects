namespace Bam.Data.Objects;

public interface IObjectDataReadResult
{
    IObjectData ObjectData { get; }
    string Message { get; set; }
    bool Success { get; set; }
}