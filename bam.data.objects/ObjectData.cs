using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Bam.Data.Dynamic.Objects;
using Bam.Net;
using Bam.Net.Data;
using Bam.Storage;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

public class ObjectData : IObjectData
{
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

    private Dictionary<string, IObjectProperty> _propertyDictionary;
    public IObjectProperty? Property(string propertyName)
    {
        if (_propertyDictionary == null)
        {
            _propertyDictionary = new Dictionary<string, IObjectProperty>();
            foreach (IObjectProperty property in Properties)
            {
                _propertyDictionary.Add(property.PropertyName, property);
            }
        }

        return _propertyDictionary.GetValueOrDefault(propertyName);
    }

    public IObjectData? Property(string propertyName, object value)
    {
        IObjectProperty? property = Property(propertyName);
        if (property != null)
        {
            property.SetValue(this.Data, value);
        }

        return this;
    }

    private IEnumerable<IObjectProperty> _properties;
    public IEnumerable<IObjectProperty> Properties
    {
        get
        {
            if (_properties != null)
            {
                foreach (IObjectProperty prop in _properties)
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

    public ulong GetHashId(IHashCalculator hashCalculator)
    {
        return hashCalculator.CalculateHash(this);
    }

    public ulong GetKeyHashId(IKeyHashCalculator keyHashCalculator)
    {
        return keyHashCalculator.CalculateKeyHash(this);
    }

    private IEnumerable<IObjectProperty> GetObjectProperties()
    {
        foreach (PropertyInfo propertyInfo in Type.Type.GetProperties())
        {
            DataTypes enumType = DataTypeTranslator.EnumFromType(propertyInfo.PropertyType);
            if (enumType != DataTypes.Default)
            {
                yield return new ObjectProperty(this, propertyInfo.Name, propertyInfo.GetValue(this.Data));
            }
        }
    }
}