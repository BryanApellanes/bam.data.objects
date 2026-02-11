namespace Bam.Data.Objects;

public interface IObjectDataSearchIndexResult
{
    bool Success { get; set; }
    string Message { get; set; }
    int PropertiesIndexed { get; set; }
}
