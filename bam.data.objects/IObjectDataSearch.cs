namespace Bam.Data.Objects;

/// <summary>
/// Defines a search query targeting a specific type with one or more search criteria.
/// </summary>
public interface IObjectDataSearch
{
    /// <summary>
    /// Gets the type of objects to search for.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the collection of search criteria to apply.
    /// </summary>
    IEnumerable<ObjectDataSearchCriterion> Criteria { get; }
}
