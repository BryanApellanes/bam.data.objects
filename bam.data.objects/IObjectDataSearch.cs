namespace Bam.Data.Objects;

public interface IObjectDataSearch
{
    Type Type { get; }
    IEnumerable<ObjectDataSearchCriterion> Criteria { get; }
}
