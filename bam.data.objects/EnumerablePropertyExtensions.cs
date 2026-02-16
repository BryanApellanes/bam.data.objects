using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

/// <summary>
/// Provides extension methods for reconstructing objects from collections of <see cref="IProperty"/> instances.
/// </summary>
public static class EnumerablePropertyExtensions
{
    /// <summary>
    /// Reconstructs a strongly-typed object from a collection of properties by setting each property value on a new instance.
    /// </summary>
    /// <typeparam name="T">The type to reconstruct.</typeparam>
    /// <param name="properties">The properties to use for reconstruction.</param>
    /// <returns>The reconstructed object, or null if the type cannot be resolved.</returns>
    public static T? FromObjectProperties<T>(this IEnumerable<IProperty> properties)
    {
        return (T)FromObjectProperties(properties);
    }

    /// <summary>
    /// Reconstructs an object from a collection of properties by resolving the type from <see cref="IProperty.AssemblyQualifiedTypeName"/> and setting each property value on a new instance.
    /// </summary>
    /// <param name="properties">The properties to use for reconstruction. All properties must share the same type name.</param>
    /// <returns>The reconstructed object, or null if the type cannot be resolved.</returns>
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