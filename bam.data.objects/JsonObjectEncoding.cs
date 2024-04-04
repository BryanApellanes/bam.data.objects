using System.Text;
using Bam.Net;
using Bam.Storage;

namespace Bam.Data.Objects;

public class JsonObjectEncoding : RawData, IObjectEncoding
{
    public JsonObjectEncoding(byte[] value, Encoding encoding = null): base(value, encoding)
    {
    }
    
    public virtual Type Type { get; set; }

    public virtual object ToObject()
    {
        return Encoding.GetString(Value).FromJson(Type);
    } 
}