namespace Bam.Data.Objects;

public interface IObjectDataSearcher
{
    Task<IObjectDataSearchResult> SearchAsync(IObjectDataSearch dataSearch);
}