using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

public class ObjectPropertyReader : IObjectPropertyReader
{
    public IObjectPropertyReadResult ReadProperty(Type type, PropertyInfo property)
    {
        throw new NotImplementedException();
    }

    public IObjectPropertyReadResult<TValue> ReadProperty<TValue>(Type type, PropertyInfo property)
    {
        throw new NotImplementedException();
    }
}