namespace Bam.Data.Objects;

public interface IObjectDataIndexResult
{
    bool Success { get; }
    ulong Id { get; }
    IObjectDataKey ObjectDataKey { get; }
}
