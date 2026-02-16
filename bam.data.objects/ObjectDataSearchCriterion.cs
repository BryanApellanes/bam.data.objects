namespace Bam.Data.Objects;

/// <summary>
/// Represents a single search criterion specifying a property name, value, and comparison operator.
/// </summary>
public class ObjectDataSearchCriterion
{
    /// <summary>
    /// Gets or sets the name of the property to filter on.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the value to compare the property against.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets the comparison operator to use for this criterion.
    /// </summary>
    public SearchOperator Operator { get; set; }
}
