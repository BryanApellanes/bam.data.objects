using System.Text;
using Bam.Data.Objects;

namespace Bam.Storage;

public class ObjectData<T>: ObjectData, IObjectData<T>
{
    public ObjectData(object data) : base(data)
    {
    }

    public ObjectData(object data, Encoding encoding) : base(data, encoding)
    {
    }

}