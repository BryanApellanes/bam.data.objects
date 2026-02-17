using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Strongly-typed wrapper around an object of type <typeparamref name="T"/>, extending <see cref="ObjectData"/> with type-safe access.
/// </summary>
/// <typeparam name="T">The type of the underlying data object.</typeparam>
public class ObjectData<T>: ObjectData, IObjectData<T>
{
    /// <summary>
    /// Implicitly converts an <see cref="ObjectData{T}"/> to its underlying data of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="data">The object data wrapper to convert.</param>
    public static implicit operator T(ObjectData<T> data)
    {
        return (T)data.Data;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectData{T}"/> class wrapping the specified data.
    /// </summary>
    /// <param name="data">The data object to wrap.</param>
    public ObjectData(T data) : base(data!)
    {
        this.Data = data;
    }

    private T _data = default!;
    /// <summary>
    /// Gets or sets the strongly-typed underlying data object, casting from the base <see cref="ObjectData.Data"/> if needed.
    /// </summary>
    public new T Data
    {
        get
        {
            if (_data != null)
            {
                return _data;
            }
            if(base.Data != null && base.Data.TryCast<T>(out T obj))
            {
                _data = obj;
            }

            return _data;
        }
        set
        {
            _data = value;
            base.Data = value!;
        }
    }
}