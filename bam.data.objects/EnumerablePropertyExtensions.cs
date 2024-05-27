using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam;

namespace Bam.Data.Objects;

public static class EnumerablePropertyExtensions
{
    public static T? FromObjectProperties<T>(this IEnumerable<IProperty> properties)
    {
        return (T)FromObjectProperties(properties);
    }
    
    public static object? FromObjectProperties(this IEnumerable<IProperty> properties)
    {
        EnsureMatchingTypeNames(properties);
        Type type = Type.GetType(properties.First().AssemblyQualifiedTypeName);
        if (type == null)
        {
            return null;
        }
        object data = type.Construct();
        foreach (Property property in properties)
        {
            PropertyInfo propertyInfo = type.GetProperty(property.PropertyName);

            object value = property.Value.FromJson(propertyInfo.PropertyType);
            
            propertyInfo.SetValue(data, value);
        }

        return data;
    }
    
    private static void EnsureMatchingTypeNames(IEnumerable<IProperty> properties)
    {
        string typeName = null;
        foreach (Property property in properties)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                typeName = property.AssemblyQualifiedTypeName;
            }

            if (!property.AssemblyQualifiedTypeName.Equals(typeName))
            {
                throw new InvalidOperationException("TypeNames must match");
            }
        }
    }
}