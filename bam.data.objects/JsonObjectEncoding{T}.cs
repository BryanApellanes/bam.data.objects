using System.Text;
using Bam.Net;

namespace Bam.Data.Dynamic.Objects;

public class JsonObjectEncoding<T> : JsonObjectEncoding
{
    public JsonObjectEncoding(byte[] value, Encoding encoding = null): base(value, encoding)
    {
    }
    
    public JsonObjectEncoding(T data, Encoding encoding = null) : base(data, encoding)
    {
    }
    
    public override Type Type => typeof(T);

    public virtual T ToObject()
    {
        return (T)base.ToObject();
    }
}