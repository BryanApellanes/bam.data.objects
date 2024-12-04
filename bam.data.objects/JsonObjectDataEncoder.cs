using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam;
using MongoDB.Bson;

namespace Bam.Data.Objects;

public class JsonObjectDataEncoder : ObjectDataEncoder
{
    public JsonObjectDataEncoder(): this(Encoding.UTF8)
    {
    }
    
    public JsonObjectDataEncoder(Encoding encoding)
    {
        this.Encoding = encoding ?? Encoding.UTF8;
    }

    private Encoding Encoding { get; set; }
    
    public override string Stringify(object data)
    {
        if (data == null)
        {
            return "null";
        }
        return data.ToJson();
    }

    public override object Objectify(string data)
    {
        return Objectify<Dictionary<string, object>>(data);
    }

    public override T Objectify<T>(string data)
    {
        return data.FromJson<T>(); }

    public override IObjectEncoding Encode(object data)
    {
        string json = string.Empty;
        if (data is IJsonable jsonable)
        {
            json = jsonable.ToJson();
        }
        else
        {
            json = data.ToJson();
        }
        return new JsonObjectEncoding(Encoding.GetBytes(json), Encoding){Type = data.GetType()};
    }

    public override object Decode(byte[] encoding, Type type)
    {
        JsonObjectEncoding json = new JsonObjectEncoding(encoding)
        {
            Type = type
        };
        return json.ToObject();
    }

    public override object Decode(IObjectEncoding encoding)
    {
        if (encoding is JsonObjectEncoding jsonEncoding)
        {
            return jsonEncoding.ToObject();
        }

        return null;
    }

    public override T Decode<T>(byte[] encoding)
    {
        JsonObjectEncoding<T> jsonEncoding = new JsonObjectEncoding<T>(encoding);
        return jsonEncoding.ToObject();
    }

    public override T Decode<T>(IObjectEncoding<T> encoding)
    {
        if (encoding is JsonObjectEncoding<T> jsonEncoding)
        {
            return jsonEncoding.ToObject();
        }

        return default;
    }
}