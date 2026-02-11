namespace Bam.Data.Objects;

public interface IObjectDataDeleteResult
{
    bool Success { get; set; }
    string Message { get; set; }
}