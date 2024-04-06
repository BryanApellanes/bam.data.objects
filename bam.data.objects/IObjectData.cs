using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectData 
{
    TypeDescriptor Type { get; }
    IEnumerable<IObjectProperty> Properties { get; }
    string ToJson();
}