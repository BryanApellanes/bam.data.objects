using System.Reflection;
using Bam.Net;

namespace Bam.Data.Dynamic.Objects;

public static class ObjectExtensions
{
    
    public static IEnumerable<ObjectProperty> ToObjectProperties(this object data)
    {
        Args.ThrowIfNull(data, nameof(data));
        Type type = data.GetType();
        foreach (PropertyInfo property in type.GetProperties().Where(prop => prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTimeOffset)))
        {
            yield return new ObjectProperty(type, property, property.GetValue(data));
        }
    }
}