using System.Reflection;
using System.Text;
using Bam.Data.Objects;
using Bam.Net;
using Bam.Storage;
using Microsoft.CodeAnalysis;

namespace Bam.Data.Dynamic.Objects;

public class ObjectProperty : IObjectProperty
{
    public ObjectProperty(IObjectData parent, string propertyName, object propertyValue)
    {
        this.Parent = parent;
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.ObjectEncoding = this.ObjectEncoder.Encode(propertyValue);
        this.AssemblyQualifiedTypeName = parent.Type.Type.AssemblyQualifiedName;
        this.PropertyName = propertyName;
        this.Value = Encoding.UTF8.GetString(ObjectEncoding.Value) ?? "null";
    }
    
    protected ObjectEncoder ObjectEncoder
    {
        get;
        set;
    }

    protected IObjectEncoding ObjectEncoding
    {
        get;
        set;
    }

    public object Decode()
    {
        return ObjectEncoder.Decode(ObjectEncoding);
    }
    
    public IObjectData Parent { get; set; }

    /// <summary>
    /// Gets or sets the AssemblyQualifiedName of the type this property belongs to.
    /// </summary>
    public string AssemblyQualifiedTypeName { get; set; }
    
    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string PropertyName { get; set; }
    
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; }

    public object SetValue(object target)
    {
        Type type = Type.GetType(AssemblyQualifiedTypeName);
        PropertyInfo property = type.GetProperty(PropertyName);
        property.SetValue(target, Decode());
        return target;
    }

    public object SetValue(object target, object value)
    {
        this.ObjectEncoding = this.ObjectEncoder.Encode(value);
        this.Value = Encoding.UTF8.GetString(ObjectEncoding.Value) ?? "null";
        return SetValue(target);
    }
    
    public IRawData ToRawData(Encoding encoding = null)
    {
        return new RawData(this.ObjectEncoder.Encode(this).Value, encoding);
    }

    public static ObjectProperty FromData(PropertyInfo property, object data)
    {
        IObjectData parent = new ObjectData(data);
        if (data is IObjectData dataParent)
        {
            parent = dataParent;
        }
        return new ObjectProperty(parent, property.Name, property.GetValue(data));
    }

    public string ToJson()
    {
        return Value;
    }
}