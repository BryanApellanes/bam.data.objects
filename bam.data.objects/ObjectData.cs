using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam.Net;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectData : RawData, IObjectData
{
    public ObjectData(object data) : base(data)
    {
    }

    public ObjectData(object data, Encoding encoding) : base(data, encoding)
    {
    }

    public virtual Type Type { get; init; }
    public IEnumerable<IObjectProperty> Properties { get; }
}