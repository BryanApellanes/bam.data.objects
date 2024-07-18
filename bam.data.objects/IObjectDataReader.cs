namespace Bam.Data.Objects;

public interface IObjectDataReader
{
    Task<IObjectDataReadResult> ReadObjectDataAsync(IObjectKey key);
}