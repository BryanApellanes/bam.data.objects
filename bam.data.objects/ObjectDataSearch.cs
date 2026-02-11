namespace Bam.Data.Objects;

public class ObjectDataSearch : IObjectDataSearch
{
    public ObjectDataSearch()
    {
        this.Criteria = new List<ObjectDataSearchCriterion>();
    }

    public ObjectDataSearch(Type type) : this()
    {
        this.Type = type;
    }

    public Type Type { get; set; }
    public IList<ObjectDataSearchCriterion> Criteria { get; set; }

    IEnumerable<ObjectDataSearchCriterion> IObjectDataSearch.Criteria => Criteria;

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
