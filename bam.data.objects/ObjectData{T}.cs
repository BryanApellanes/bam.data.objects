using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectData<T>: ObjectData, IObjectData<T>
{
    public static implicit operator T(ObjectData<T> data)
    {
        return (T)data.Data;
    }
    
    public ObjectData(T data) : base(data)
    {
        this.Data = data;
    }

    private T _data;
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
            base.Data = value;
        }
    }
}