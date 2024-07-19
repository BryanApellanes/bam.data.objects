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
    public ObjectData(object data) : base()
    {
        this.Data = data;
        this.Type = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }

    internal ObjectData(object data, IObjectEncoderDecoder encoder)
    {
        this.Data = data;
        this.Type = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = encoder;
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }
    
    [JsonIgnore]
    [YamlIgnore]
    public virtual object Data
    {
        get;
        set;
    }
    
    protected IObjectEncoderDecoder ObjectEncoder
    {
        get;
        set;
    }

    protected IDataTypeTranslator DataTypeTranslator
    {
        get;
        set;
    }
    
    public IObjectDataIdentifierFactory ObjectDataIdentifierFactory { get; set; }

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

    /// <summary>
    /// Sets the value of the specified property to the specified value.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
    
    public IObjectDataKey GetObjectKey()
    {
        Args.ThrowIfNull(this.ObjectDataIdentifierFactory, nameof(ObjectDataIdentifierFactory));
        return this.ObjectDataIdentifierFactory.GetObjectKey(this);
    }

    public IObjectDataIdentifier GetObjectIdentifier()
    {
        Args.ThrowIfNull(this.ObjectDataIdentifierFactory, nameof(ObjectDataIdentifierFactory));
        return this.ObjectDataIdentifierFactory.GetObjectIdentifier(this);
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