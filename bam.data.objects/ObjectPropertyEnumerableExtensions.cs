using System.Reflection;
using Bam.Net;

namespace Bam.Data.Dynamic.Objects;

public static class ObjectPropertyEnumerableExtensions
{
    public static T? FromObjectProperties<T>(this IEnumerable<ObjectProperty> properties)
    {
        return (T)FromObjectProperties(properties);
    }
    
    public static object? FromObjectProperties(this IEnumerable<ObjectProperty> properties)
    {
        EnsureMatchingTypeNames(properties);
        Type type = Type.GetType(properties.First().TypeName);
        if (type == null)
        {
            return null;
        }
        object data = type.Construct();
        foreach (ObjectProperty property in properties)
        {
            PropertyInfo propertyInfo = type.GetProperty(property.PropertyName);

            object value = property.Value.FromJson(propertyInfo.PropertyType);
            
            propertyInfo.SetValue(data, value);
        }

        return data;
    }
    
    private static void EnsureMatchingTypeNames(IEnumerable<ObjectProperty> properties)
    {
        string typeName = null;
        foreach (ObjectProperty property in properties)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                typeName = property.TypeName;
            }

            if (!property.TypeName.Equals(typeName))
            {
                throw new InvalidOperationException("TypeNames must match");
            }
        }
    }
}