using System.Text;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a JSON-encoded object as raw bytes, providing deserialization back to the original object type.
/// </summary>
public class JsonObjectEncoding : RawData, IObjectEncoding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectEncoding"/> class from the specified byte array.
    /// </summary>
    /// <param name="value">The JSON-encoded bytes.</param>
    /// <param name="encoding">The text encoding to use, or null for the default.</param>
    public JsonObjectEncoding(byte[] value, Encoding encoding = null!): base(value, encoding)
    {
    }

    /// <summary>
    /// Gets or sets the CLR type to deserialize the JSON bytes into.
    /// </summary>
    public virtual Type Type { get; set; } = null!;

    /// <summary>
    /// Deserializes the JSON bytes back to an object of the specified <see cref="Type"/>.
    /// </summary>
    /// <returns>The deserialized object.</returns>
    public virtual object ToObject()
    {
        return Encoding.GetString(Value).FromJson(Type);
    } 
}