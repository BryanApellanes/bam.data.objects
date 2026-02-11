namespace Bam.Data.Objects;

public class ObjectDataSearchCriterion
{
    public string PropertyName { get; set; }
    public object Value { get; set; }
    public SearchOperator Operator { get; set; }
}
