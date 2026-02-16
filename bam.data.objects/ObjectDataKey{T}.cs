
using bam.data.objects;

namespace Bam.Data.Objects;

/// <summary>
/// Strongly-typed implementation of <see cref="ObjectDataKey"/> that automatically sets the type descriptor from <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the identified object.</typeparam>
public class ObjectDataKey<T> : ObjectDataKey, IObjectDataKey<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataKey{T}"/> class with the type descriptor set to <typeparamref name="T"/>.
    /// </summary>
    public ObjectDataKey()
    {
        this.TypeDescriptor = typeof(T);
    }

    /// <inheritdoc />
    public TypeDescriptor TypeDescriptor { get; }

    /// <inheritdoc />
    public string Key { get; }
}