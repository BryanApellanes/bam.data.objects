using System.Text;
using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Represents a strongly-typed JSON-encoded object as raw bytes, providing deserialization back to type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the encoded object.</typeparam>
public class JsonObjectEncoding<T> : JsonObjectEncoding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectEncoding{T}"/> class from the specified byte array.
    /// </summary>
    /// <param name="value">The JSON-encoded bytes.</param>
    /// <param name="encoding">The text encoding to use, or null for the default.</param>
    public JsonObjectEncoding(byte[] value, Encoding encoding = null): base(value, encoding)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectEncoding{T}"/> class by serializing the data to JSON bytes.
    /// </summary>
    /// <param name="data">The data object to encode.</param>
    /// <param name="encoding">The text encoding to use, or null for the default.</param>
    public JsonObjectEncoding(T data, Encoding encoding = null) : base(encoding.GetBytes(data.ToJson()), encoding)
    {
    }

    /// <inheritdoc />
    public override Type Type => typeof(T);

    /// <summary>
    /// Deserializes the JSON bytes back to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The deserialized instance.</returns>
    public virtual T ToObject()
    {
        return (T)base.ToObject();
    }
}