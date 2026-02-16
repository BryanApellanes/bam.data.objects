using System.Reflection;
using System.Text.Json.Serialization;
using Bam.Data.Dynamic.Objects;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectData"/> that wraps an arbitrary object and provides property-level access, encoding, and identity operations.
/// </summary>
public class ObjectData : IObjectData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectData"/> class wrapping the specified data object.
    /// </summary>
    /// <param name="data">The data object to wrap.</param>
    public ObjectData(object data) : base()
    {
        this.Data = data;
        this.TypeDescriptor = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = JsonObjectDataEncoder.Default;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectData"/> class by constructing a new instance of the type described by the specified type descriptor.
    /// </summary>
    /// <param name="typeDescriptor">The type descriptor whose type to construct and wrap.</param>
    public ObjectData(TypeDescriptor typeDescriptor) : base()
    {
        this.Data = typeDescriptor.Type.Construct();
        this.TypeDescriptor = typeDescriptor;
        this.ObjectEncoder = JsonObjectDataEncoder.Default;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }

    internal ObjectData(object data, IObjectEncoderDecoder encoder)
    {
        this.Data = data;
        this.TypeDescriptor = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = encoder;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }
    
    /// <inheritdoc />
    [JsonIgnore]
    [YamlIgnore]
    public object Data
    {
        get;
        set;
    }

    private IObjectEncoderDecoder ObjectEncoder
    {
        get;
        set;
    }

    private IDataTypeTranslator DataTypeTranslator
    {
        get;
        set;
    }
    
    /// <inheritdoc />
    public IObjectDataLocatorFactory ObjectDataLocatorFactory { get; set; }

    /// <inheritdoc />
    public TypeDescriptor TypeDescriptor
    {
        get;
        set;
    }

    private Dictionary<string, IProperty> _propertyDictionary;

    /// <inheritdoc />
    public IProperty? Property(string propertyName)
    {
        if (_propertyDictionary == null)
        {
            _propertyDictionary = new Dictionary<string, IProperty>();
            foreach (IProperty property in Properties)
            {
                _propertyDictionary.Add(property.PropertyName, property);
            }
        }

        return _propertyDictionary.GetValueOrDefault(propertyName);
    }

    /// <inheritdoc />
    public IObjectData? Property(string propertyName, object value)
    {
        IProperty? property = Property(propertyName);
        if (property != null)
        {
            property.SetValue(this.Data, value);
        }

        return this;
    }

    private IEnumerable<IProperty> _properties;

    /// <inheritdoc />
    public IEnumerable<IProperty> Properties
    {
        get
        {
            if (_properties != null)
            {
                foreach (IProperty prop in _properties)
                {
                    yield return prop;
                }
            }
            else if (TypeDescriptor != null && TypeDescriptor.Type != null)
            {
                _properties = GetObjectProperties();
                foreach (var objectProperty in _properties) yield return objectProperty;
            }
        }
        set => _properties = value;
    }

    /// <summary>
    /// Serializes the underlying data object to a JSON string.
    /// </summary>
    /// <returns>The JSON representation of the data.</returns>
    public string ToJson()
    {
        return Data.ToJson();
    }

    /// <inheritdoc />
    public IObjectEncoding Encode()
    {
        return ObjectEncoder.Encode(this.Data);
    }
    
    /// <inheritdoc />
    public IObjectDataKey GetObjectKey()
    {
        Args.ThrowIfNull(this.ObjectDataLocatorFactory, nameof(ObjectDataLocatorFactory));
        return this.ObjectDataLocatorFactory.GetObjectKey(this);
    }

    /// <inheritdoc />
    public IObjectDataIdentifier GetObjectIdentifier()
    {
        Args.ThrowIfNull(this.ObjectDataLocatorFactory, nameof(ObjectDataLocatorFactory));
        return this.ObjectDataLocatorFactory.GetObjectIdentifier(this);
    }

    private IEnumerable<IProperty> GetObjectProperties()
    {
        foreach (PropertyInfo propertyInfo in TypeDescriptor.Type.GetProperties())
        {
            DataTypes enumType = DataTypeTranslator.EnumFromType(propertyInfo.PropertyType);
            if (enumType != DataTypes.Default)
            {
                object propValue = propertyInfo.GetValue(this.Data);
                if (propValue != null)
                {
                    yield return new Property(this, propertyInfo.Name, propValue);
                }
            }
        }
    }
}