namespace Bam.Data.Objects;

public interface IObjectSearcher
{
    Task<IObjectDataSearchResult> SearchAsync(IObjectSearch search);
}