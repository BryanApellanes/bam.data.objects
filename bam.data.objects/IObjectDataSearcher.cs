namespace Bam.Data.Objects;

/// <summary>
/// Defines the operation for searching object data using multi-criteria search queries.
/// </summary>
public interface IObjectDataSearcher
{
    /// <summary>
    /// Searches for object data matching the specified search criteria asynchronously.
    /// </summary>
    /// <param name="dataSearch">The search definition containing type and criteria.</param>
    /// <returns>The search result containing matching objects.</returns>
    Task<IObjectDataSearchResult> SearchAsync(IObjectDataSearch dataSearch);
}