using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

/// <summary>
/// Abstract base class for encoding and decoding objects to and from byte representations.
/// </summary>
public abstract class ObjectDataEncoder: IObjectEncoderDecoder//, IObjectConverter, IObjectEncoder, IObjectDecoder
{
    private static readonly object _defaultLock = new();
    private static JsonObjectDataEncoder _default = null!;

    /// <summary>
    /// Gets the default <see cref="JsonObjectDataEncoder"/> singleton instance.
    /// </summary>
    public static JsonObjectDataEncoder Default
    {
        get
        {
            return _defaultLock.DoubleCheckLock(ref _default, () => new JsonObjectDataEncoder());
        }
    }
    
    /// <summary>
    /// Deserializes the specified string data to an object.
    /// </summary>
    /// <param name="data">The string data to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    public abstract object Objectify(string data);

    /// <summary>
    /// Deserializes the specified string data to a strongly-typed instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="data">The string data to deserialize.</param>
    /// <returns>The deserialized instance.</returns>
    public abstract T Objectify<T>(string data);

    /// <summary>
    /// Converts the specified object to its string representation.
    /// </summary>
    /// <param name="data">The object to stringify.</param>
    /// <returns>The string representation of the object.</returns>
    public abstract string Stringify(object data);

    /// <summary>
    /// Encodes the specified object to an <see cref="IObjectEncoding"/> byte representation.
    /// </summary>
    /// <param name="data">The object to encode.</param>
    /// <returns>The encoded representation.</returns>
    public abstract IObjectEncoding Encode(object data);

    /// <summary>
    /// Decodes the specified byte array to an object of the given type.
    /// </summary>
    /// <param name="encoding">The encoded bytes.</param>
    /// <param name="type">The type to decode to.</param>
    /// <returns>The decoded object.</returns>
    public abstract object Decode(byte[] encoding, Type type);

    /// <summary>
    /// Decodes the specified object encoding back to an object.
    /// </summary>
    /// <param name="encoding">The object encoding to decode.</param>
    /// <returns>The decoded object.</returns>
    public abstract object Decode(IObjectEncoding encoding);

    /// <summary>
    /// Decodes the specified byte array to a strongly-typed instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to decode to.</typeparam>
    /// <param name="encoding">The encoded bytes.</param>
    /// <returns>The decoded instance.</returns>
    public abstract T Decode<T>(byte[] encoding);

    /// <summary>
    /// Decodes the specified strongly-typed encoding to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to decode to.</typeparam>
    /// <param name="encoding">The typed encoding to decode.</param>
    /// <returns>The decoded instance.</returns>
    public abstract T Decode<T>(IObjectEncoding<T> encoding);
}