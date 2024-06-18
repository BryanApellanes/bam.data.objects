using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Bam.Data.Dynamic.Objects;
using Bam;
using Bam.Data;
using Bam.Storage;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

public class ObjectData : IObjectData
{
    internal ObjectData(Type type) // for reading
    {
        this.Type = new TypeDescriptor(type);
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }
    
    public ObjectData(object data) : base()
    {
        this.Data = data;
        this.Type = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }

    internal ObjectData(object data, ObjectEncoder encoder)
    {
        this.Data = data;
        this.Type = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = encoder;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }
    
    [JsonIgnore]
    [YamlIgnore]
    public object Data
    {
        get;
        set;
    }
    
    protected ObjectEncoder ObjectEncoder
    {
        get;
        set;
    }

    protected IDataTypeTranslator DataTypeTranslator
    {
        get;
        set;
    }

    public TypeDescriptor Type
    {
        get;
        set;
    }

    private Dictionary<string, IProperty> _propertyDictionary;
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
            if (Type != null && Type.Type != null)
            {
                _properties = GetObjectProperties();
                foreach (var objectProperty in _properties) yield return objectProperty;
            }
        }
        set => _properties = value;
    }

    public string ToJson()
    {
        return Data.ToJson();
    }

    public IObjectEncoding Encode()
    {
        return ObjectEncoder.Encode(this.Data);
    }

    public IObjectIdentifierFactory ObjectIdentifierFactory { get; set; }
    
    public IObjectKey GetObjectKey()
    {
        Args.ThrowIfNull(this.ObjectIdentifierFactory, nameof(ObjectIdentifierFactory));
        return this.ObjectIdentifierFactory.GetObjectKey(this);
    }

    public IObjectIdentifier GetObjectIdentifier()
    {
        Args.ThrowIfNull(this.ObjectIdentifierFactory, nameof(ObjectIdentifierFactory));
        return this.ObjectIdentifierFactory.GetObjectIdentifier(this);
    }

    private IEnumerable<IProperty> GetObjectProperties()
    {
        foreach (PropertyInfo propertyInfo in Type.Type.GetProperties())
        {
            DataTypes enumType = DataTypeTranslator.EnumFromType(propertyInfo.PropertyType);
            if (enumType != DataTypes.Default)
            {
                yield return new Property(this, propertyInfo.Name, propertyInfo.GetValue(this.Data));
            }
        }
    }
}