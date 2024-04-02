using System.Text;
using Bam.Net;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class JsonObjectEncoding : RawData, IObjectEncoding
{
    public JsonObjectEncoding(byte[] value, Encoding encoding = null): base(value, encoding)
    {
    }
    
    public JsonObjectEncoding(object data, Encoding encoding = null) : base(data, encoding)
    {
        Args.ThrowIfNull(data, nameof(data));
        Encoding = encoding ?? Encoding.UTF8;
        Type = data.GetType();
    }
    
    public virtual Type Type { get; set; }

    public virtual object ToObject()
    {
        return Encoding.GetString(Value).FromJson(Type);
    } 
}