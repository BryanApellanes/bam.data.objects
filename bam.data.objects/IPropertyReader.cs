using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

public interface IPropertyReader
{
    IPropertyReadResult ReadProperty(Type type, PropertyInfo property);

    IPropertyReadResult<TValue> ReadProperty<TValue>(Type type, PropertyInfo property);

}