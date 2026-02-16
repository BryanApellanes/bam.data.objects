namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataSearch"/> that builds a search query with fluent <see cref="Where"/> calls.
/// </summary>
public class ObjectDataSearch : IObjectDataSearch
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataSearch"/> class with an empty criteria list.
    /// </summary>
    public ObjectDataSearch()
    {
        this.Criteria = new List<ObjectDataSearchCriterion>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataSearch"/> class targeting the specified type.
    /// </summary>
    /// <param name="type">The type of objects to search for.</param>
    public ObjectDataSearch(Type type) : this()
    {
        this.Type = type;
    }

    /// <inheritdoc />
    public Type Type { get; set; }

    /// <summary>
    /// Gets or sets the mutable list of search criteria.
    /// </summary>
    public IList<ObjectDataSearchCriterion> Criteria { get; set; }

    /// <inheritdoc />
    IEnumerable<ObjectDataSearchCriterion> IObjectDataSearch.Criteria => Criteria;

    /// <summary>
    /// Adds a search criterion and returns this instance for fluent chaining.
    /// </summary>
    /// <param name="propertyName">The name of the property to search by.</param>
    /// <param name="value">The value to match against.</param>
    /// <param name="op">The search operator to apply. Defaults to <see cref="SearchOperator.Equals"/>.</param>
    /// <returns>This search instance for fluent chaining.</returns>
    public ObjectDataSearch Where(string propertyName, object value, SearchOperator op = SearchOperator.Equals)
    {
        Criteria.Add(new ObjectDataSearchCriterion
        {
            PropertyName = propertyName,
            Value = value,
            Operator = op
        });
        return this;
    }
}
