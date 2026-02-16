namespace Bam.Data.Objects;

/// <summary>
/// Defines the comparison operators available for object data search criteria.
/// </summary>
public enum SearchOperator
{
    /// <summary>
    /// The property value must equal the search value exactly.
    /// </summary>
    Equals,

    /// <summary>
    /// The property value must start with the search value.
    /// </summary>
    StartsWith,

    /// <summary>
    /// The property value must end with the search value.
    /// </summary>
    EndsWith,

    /// <summary>
    /// The property value must contain the search value.
    /// </summary>
    Contains,

    /// <summary>
    /// The property value must not start with the search value.
    /// </summary>
    DoesntStartWith,

    /// <summary>
    /// The property value must not end with the search value.
    /// </summary>
    DoesntEndWith,

    /// <summary>
    /// The property value must not contain the search value.
    /// </summary>
    DoesntContain
}
