using Bam.Data.Dynamic.Objects;
using Bam.Net;

namespace Bam.Data.Objects;

public abstract class ObjectEncoder: IObjectEncoderDecoder, IObjectConverter, IObjectEncoder, IObjectDecoder
{
    private static readonly object _defaultLock = new();
    private static JsonObjectEncoder _default;
    public static JsonObjectEncoder Default
    {
        get
        {
            return _defaultLock.DoubleCheckLock(ref _default, () => new JsonObjectEncoder());
        }
    }
    
    public abstract object Objectify(string data);

    public abstract T Objectify<T>(string data);

    public abstract string Stringify(object data);

    public abstract IObjectEncoding Encode(object data);

    public abstract object Decode(byte[] encoding, Type type);

    public abstract object Decode(IObjectEncoding encoding);

    public abstract T Decode<T>(byte[] encoding);

    public abstract T Decode<T>(IObjectEncoding<T> encoding);
}