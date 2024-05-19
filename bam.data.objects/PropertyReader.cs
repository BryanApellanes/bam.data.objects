using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

public class PropertyReader : IPropertyReader
{
    public IPropertyReadResult ReadProperty(Type type, PropertyInfo property)
    {
        throw new NotImplementedException();
    }

    public IPropertyReadResult<TValue> ReadProperty<TValue>(Type type, PropertyInfo property)
    {
        throw new NotImplementedException();
    }
}