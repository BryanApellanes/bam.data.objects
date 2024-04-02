using Bam.Net;

namespace Bam.Data.Dynamic.Objects;

public abstract class ObjectEncoder: IObjectConverter, IObjectEncoder, IObjectDecoder
{
    public abstract object Objectify(string data);

    public abstract T Objectify<T>(string data);

    public abstract string Stringify(object data);

    public abstract IObjectEncoding Encode(object data);

    public abstract object Decode(byte[] encoding, Type type);

    public abstract object Decode(IObjectEncoding encoding);

    public abstract T Decode<T>(byte[] encoding);

    public abstract T Decode<T>(IObjectEncoding<T> encoding);
}