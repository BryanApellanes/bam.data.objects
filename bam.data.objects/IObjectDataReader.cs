namespace Bam.Data.Objects;

public interface IObjectDataReader
{
    Task<IObjectDataReadResult> ReadObjectDataAsync(IObjectDataKey dataKey);
}