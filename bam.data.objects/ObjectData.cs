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
        this.DataTypeTranslator = Bam.Net.Data.DataTypeTranslator.Default;
    }

    public ObjectData(object data, Encoding encoding) : base()
    {
        this.Data = data;
        this.Type = new TypeDescriptor(data?.GetType());
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.DataTypeTranslator = Bam.Net.Data.DataTypeTranslator.Default;
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
                foreach (var objectProperty in GetObjectProperties()) yield return objectProperty;
            }
        }
        set => _properties = value;
    }

    public string ToJson()
    {
        return Data.ToJson();
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