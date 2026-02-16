using Bam.Data.Objects;

namespace bam.data.objects;

/// <summary>
/// Represents a strongly-typed composite-key-based identifier for an object of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the identified object.</typeparam>
public interface IObjectDataKey<T>: IObjectDataKey
{

}