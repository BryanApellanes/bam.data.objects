
using bam.data.objects;

namespace Bam.Data.Objects;

public class ObjectDataKey<T> : ObjectDataKey, IObjectDataKey<T> 
{
    public ObjectDataKey()
    {
        this.TypeDescriptor = typeof(T);
    }
    public TypeDescriptor TypeDescriptor { get; }
    public string Key { get; }
}