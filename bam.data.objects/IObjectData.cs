using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectData : IRawData
{
    Type Type { get; }
    IEnumerable<IObjectProperty> Properties { get; }
}