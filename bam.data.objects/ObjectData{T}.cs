using System.Text;
using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectData<T>: ObjectData, IObjectData<T>
{
    public static implicit operator T(ObjectData<T> data)
    {
        return (T)data.Data;
    }
    
    public ObjectData(object data) : base(data)
    {
    }

    public ObjectData(object data, Encoding encoding) : base(data, encoding)
    {
    }
}