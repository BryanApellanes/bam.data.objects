using System.Reflection;
using System.Text;
using Bam.Data.Objects;
using Bam.Storage;

//using NotImplementedException = System.NotImplementedException;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Default implementation of <see cref="IProperty"/> that represents a named, JSON-encoded property of an object with versioning and storage support.
/// </summary>
public class Property : IProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Property"/> class with the specified parent, property name, and value.
    /// </summary>
    /// <param name="parent">The parent object data that owns this property.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="propertyValue">The value of the property.</param>
    public Property(IObjectData parent, string propertyName, object propertyValue)
    {
        Args.ThrowIfNull(parent, nameof(parent));
        Args.ThrowIfNull(parent.TypeDescriptor, nameof(parent.TypeDescriptor));
        Args.ThrowIfNull(parent.TypeDescriptor.Type, nameof(parent.TypeDescriptor.Type));
        this.Parent = parent;
        this.ObjectDataEncoder = JsonObjectDataEncoder.Default;
        this.ObjectEncoding = this.ObjectDataEncoder.Encode(propertyValue);
        this.AssemblyQualifiedTypeName = parent.TypeDescriptor.Type.AssemblyQualifiedName!;
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
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string StorageSlotRelativePath
    {
        get
        {
            List<string> pathSegments = new List<string>();
            pathSegments.AddRange(Parent.TypeDescriptor.Type.Namespace!.Split('.'));
            pathSegments.Add(Parent.TypeDescriptor.Type.Name);
            pathSegments.AddRange(Parent.GetObjectKey().Key!.Split(2));
            pathSegments.Add(PropertyName);
            return Path.Combine(pathSegments.ToArray());
        }
    }

    /// <inheritdoc />
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public IEnumerable<IPropertyRevision> Versions { get; set; } = null!;

    /// <inheritdoc />
    public object Decode()
    {
        return ObjectDataEncoder.Decode(ObjectEncoding);
    }
    
    /// <inheritdoc />
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public IObjectData Parent { get; set; }

    /// <summary>
    /// Gets or sets the AssemblyQualifiedName of the type this property belongs to.
    /// </summary>
    public string AssemblyQualifiedTypeName { get; set; } = null!;

    /// <inheritdoc />
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Type Type { get; set; }

    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string PropertyName { get; set; }
    
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc />
    public object SetValue(object target)
    {
        Type type = Type.GetType(AssemblyQualifiedTypeName)!;
        PropertyInfo property = type!.GetProperty(PropertyName)!;
        property!.SetValue(target, Decode());
        return target;
    }

    /// <inheritdoc />
    public object SetValue(object target, object value)
    {
        this.ObjectEncoding = this.ObjectDataEncoder.Encode(value);
        this.Value = Encoding.UTF8.GetString(ObjectEncoding.Value) ?? "null";
        return SetValue(target);
    }
    
    /// <inheritdoc />
    public IRawData ToRawData(Encoding encoding = null!)
    {
        return new RawData(this.ObjectDataEncoder.Encode(this).Value, encoding);
    }

    /// <inheritdoc />
    public IRawData ToRawDataPointer(Encoding encoding = null!)
    {
        return new RawData(ToRawData().HashHexString);
    }

    /// <summary>
    /// Creates a <see cref="Property"/> instance from a <see cref="PropertyInfo"/> and the data object it belongs to.
    /// </summary>
    /// <param name="property">The property metadata.</param>
    /// <param name="data">The data object to read the property value from.</param>
    /// <returns>A new property instance.</returns>
    public static Property FromData(PropertyInfo property, object data)
    {
        IObjectData parent = new ObjectData(data);
        if (data is IObjectData dataParent)
        {
            parent = dataParent;
        }
        return new Property(parent, property.Name, property.GetValue(data)!);
    }

    /// <summary>
    /// Reconstructs a <see cref="Property"/> from raw data by decoding the bytes using the specified decoder and property descriptor.
    /// </summary>
    /// <param name="parent">The parent object data that owns the property.</param>
    /// <param name="objectDecoder">The decoder used to deserialize the raw data.</param>
    /// <param name="propertyDescriptor">The descriptor identifying the property name and type.</param>
    /// <param name="rawData">The raw data bytes to decode.</param>
    /// <returns>A new property instance with the decoded value.</returns>
    public static Property FromRawData(IObjectData parent, IObjectDecoder objectDecoder, IPropertyDescriptor propertyDescriptor, IRawData rawData)
    {
        return new Property(parent, propertyDescriptor.PropertyName,
            objectDecoder.Decode(rawData.Value, propertyDescriptor.PropertyType));
    }
    
    /// <inheritdoc />
    public IPropertyDescriptor ToDescriptor()
    {
        return new PropertyDescriptor(this);
    }
    
    /// <summary>
    /// Returns the JSON-serialized value of this property.
    /// </summary>
    /// <returns>The JSON value string.</returns>
    public string ToJson()
    {
        return Value;
    }
}