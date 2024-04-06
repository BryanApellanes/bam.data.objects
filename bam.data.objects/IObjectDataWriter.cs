namespace Bam.Data.Objects;

public interface IObjectDataWriter
{
    Task<IObjectDataWriteResult> WriteAsync(object data);
    Task<IObjectDataWriteResult> WriteAsync(IObjectData data);
}