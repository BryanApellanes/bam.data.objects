using System.Text;
using Bam.Data.Dynamic.Objects;
using Newtonsoft.Json.Converters;

namespace Bam.Data.Objects;

/// <summary>
/// JSON-based implementation of <see cref="ObjectDataEncoder"/> that serializes and deserializes objects using JSON.
/// </summary>
public class JsonObjectDataEncoder : ObjectDataEncoder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectDataEncoder"/> class with UTF-8 encoding.
    /// </summary>
    public JsonObjectDataEncoder(): this(Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectDataEncoder"/> class with the specified encoding.
    /// </summary>
    /// <param name="encoding">The text encoding to use for JSON serialization, or null for UTF-8.</param>
    public JsonObjectDataEncoder(Encoding encoding)
    {
        this.Encoding = encoding ?? Encoding.UTF8;
    }

    private Encoding Encoding { get; set; }
    
    /// <summary>
    /// Converts the specified object to its JSON string representation.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The JSON string, or "null" if the object is null.</returns>
    public override string Stringify(object data)
    {
        if (data == null)
        {
            return "null";
        }
        return data.ToJson(new StringEnumConverter());
    }

    /// <summary>
    /// Deserializes the specified JSON string to a dictionary of string keys and object values.
    /// </summary>
    /// <param name="data">The JSON string to deserialize.</param>
    /// <returns>The deserialized object as a dictionary.</returns>
    public override object Objectify(string data)
    {
        return Objectify<Dictionary<string, object>>(data);
    }

    /// <summary>
    /// Deserializes the specified JSON string to an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="data">The JSON string to deserialize.</param>
    /// <returns>The deserialized instance.</returns>
    public override T Objectify<T>(string data)
    {
        return data.FromJson<T>();
    }

    /// <summary>
    /// Encodes the specified object as a JSON byte array wrapped in a <see cref="JsonObjectEncoding"/>.
    /// </summary>
    /// <param name="data">The object to encode.</param>
    /// <returns>The JSON-encoded representation of the object.</returns>
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

    /// <summary>
    /// Decodes the specified JSON byte array to an object of the specified type.
    /// </summary>
    /// <param name="encoding">The JSON-encoded bytes.</param>
    /// <param name="type">The type to deserialize to.</param>
    /// <returns>The deserialized object.</returns>
    public override object Decode(byte[] encoding, Type type)
    {
        JsonObjectEncoding json = new JsonObjectEncoding(encoding)
        {
            Type = type
        };
        return json.ToObject();
    }

    /// <summary>
    /// Decodes the specified object encoding back to its original object.
    /// </summary>
    /// <param name="encoding">The object encoding to decode.</param>
    /// <returns>The deserialized object, or null if the encoding is not a <see cref="JsonObjectEncoding"/>.</returns>
    public override object Decode(IObjectEncoding encoding)
    {
        if (encoding is JsonObjectEncoding jsonEncoding)
        {
            return jsonEncoding.ToObject();
        }

        return null!;
    }

    /// <summary>
    /// Decodes the specified JSON byte array to a strongly-typed instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="encoding">The JSON-encoded bytes.</param>
    /// <returns>The deserialized instance.</returns>
    public override T Decode<T>(byte[] encoding)
    {
        JsonObjectEncoding<T> jsonEncoding = new JsonObjectEncoding<T>(encoding);
        return jsonEncoding.ToObject();
    }

    /// <summary>
    /// Decodes the specified strongly-typed object encoding back to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="encoding">The object encoding to decode.</param>
    /// <returns>The deserialized instance, or default if the encoding is not a <see cref="JsonObjectEncoding{T}"/>.</returns>
    public override T Decode<T>(IObjectEncoding<T> encoding)
    {
        if (encoding is JsonObjectEncoding<T> jsonEncoding)
        {
            return jsonEncoding.ToObject();
        }

        return default!;
    }
}