using System.Reflection;
using Bam.Data.Dynamic;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectPropertyReader
{
    IObjectPropertyReadResult ReadProperty(Type type, PropertyInfo property);

    IObjectPropertyReadResult<TValue> ReadProperty<TValue>(Type type, PropertyInfo property);

}