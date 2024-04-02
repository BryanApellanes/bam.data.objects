using System.Reflection;
using System.Text;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class ObjectProperty : IObjectProperty
{
    public ObjectProperty(PropertyInfo property, object propertyValue): this(property.DeclaringType, property, propertyValue)
    {
    }

    public ObjectProperty(Type type, PropertyInfo propertyInfo, object propertyValue)
    {
        this.TypeName = type.AssemblyQualifiedName;
        this.PropertyName = propertyInfo.Name;
        this.Value = propertyValue?.ToJson() ?? "null";
    }
    
    public string ParentHash { get; set; }
    /// <summary>
    /// Gets or sets the AssemblyQualifiedName of the type this property belongs to.
    /// </summary>
    public string TypeName { get; set; }
    
    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string PropertyName { get; set; }
    
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; }

    public IRawData ToRawData(Encoding encoding = null)
    {
        return new RawData(this, encoding);
    }
}