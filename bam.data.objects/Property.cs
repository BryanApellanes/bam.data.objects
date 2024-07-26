using System.Reflection;
using System.Text;
using Bam.Data.Objects;
using Bam;
using Bam.Storage;
using Microsoft.CodeAnalysis;
//using NotImplementedException = System.NotImplementedException;

namespace Bam.Data.Dynamic.Objects;

public class Property : IProperty
{
    public Property(IObjectData parent, string propertyName, object propertyValue)
    {
        Args.ThrowIfNull(parent, nameof(parent));
        Args.ThrowIfNull(parent.TypeDescriptor, nameof(parent.TypeDescriptor));
        Args.ThrowIfNull(parent.TypeDescriptor.Type, nameof(parent.TypeDescriptor.Type));
        this.Parent = parent;
        this.ObjectDataEncoder = JsonObjectDataEncoder.Default;
        this.ObjectEncoding = this.ObjectDataEncoder.Encode(propertyValue);
        this.AssemblyQualifiedTypeName = parent.TypeDescriptor.Type.AssemblyQualifiedName;
        this.PropertyName = propertyName;
        PropertyInfo propertyInfo = parent.TypeDescriptor.Type.GetProperty(PropertyName)!;
        this.Type = propertyInfo?.PropertyType ?? typeof(object);
        this.Value = Encoding.UTF8.GetString(ObjectEncoding.Value) ?? "null";
    }
    
    protected ObjectDataEncoder ObjectDataEncoder
    {
        get;
        set;
    }

    protected IObjectEncoding ObjectEncoding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the unversioned relative storage slot path of this property.
    /// </summary>
    public string StorageSlotRelativePath
    {
        get
        {
            List<string> pathSegments = new List<string>();
            pathSegments.AddRange(Parent.TypeDescriptor.Type.Namespace.Split('.'));
            pathSegments.Add(Parent.TypeDescriptor.Type.Name);
            pathSegments.AddRange(Parent.GetObjectKey().Key.Split(2));
            pathSegments.Add(PropertyName);
            return Path.Combine(pathSegments.ToArray());
        }
    }

    public IEnumerable<IPropertyVersion> Versions { get; set; }

    public object Decode()
    {
        return ObjectDataEncoder.Decode(ObjectEncoding);
    }
    
    public IObjectData Parent { get; set; }

    /// <summary>
    /// Gets or sets the AssemblyQualifiedName of the type this property belongs to.
    /// </summary>
    public string AssemblyQualifiedTypeName { get; set; }

    public Type Type { get; set; }

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
        this.ObjectEncoding = this.ObjectDataEncoder.Encode(value);
        this.Value = Encoding.UTF8.GetString(ObjectEncoding.Value) ?? "null";
        return SetValue(target);
    }
    
    public IRawData ToRawData(Encoding encoding = null)
    {
        return new RawData(this.ObjectDataEncoder.Encode(this).Value, encoding);
    }

    public IRawData ToRawDataPointer(Encoding encoding = null)
    {
        return new RawData(ToRawData().HashHexString);
    }

    public static Property FromData(PropertyInfo property, object data)
    {
        IObjectData parent = new ObjectData(data);
        if (data is IObjectData dataParent)
        {
            parent = dataParent;
        }
        return new Property(parent, property.Name, property.GetValue(data));
    }

    public static Property FromRawData(IObjectData parent, IObjectDecoder objectDecoder, IPropertyDescriptor propertyDescriptor, IRawData rawData)
    {
        return new Property(parent, propertyDescriptor.PropertyName,
            objectDecoder.Decode(rawData.Value, propertyDescriptor.PropertyType));
    }
    
    public IPropertyDescriptor ToDescriptor()
    {
        return new PropertyDescriptor(this);
    }
    
    public string ToJson()
    {
        return Value;
    }
}