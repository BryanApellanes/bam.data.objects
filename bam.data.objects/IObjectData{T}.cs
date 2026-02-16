using Bam.Data.Objects;

namespace Bam.Storage;

/// <summary>
/// Represents a strongly-typed wrapper around an object of type <typeparamref name="T"/> that provides property-level access, encoding, and identity operations.
/// </summary>
/// <typeparam name="T">The type of the underlying data object.</typeparam>
public interface IObjectData<T> : IObjectData
{

}