namespace Bam.Data.Dynamic.Objects;

public interface IObjectPropertyWriteResult
{
    IObjectProperty ObjectProperty { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
}