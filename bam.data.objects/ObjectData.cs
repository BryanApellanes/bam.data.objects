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
        this.Type = data?.GetType();
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.DataTypeTranslator = Bam.Net.Data.DataTypeTranslator.Default;
    }

    public ObjectData(object data, Encoding encoding) : base()
    {
        this.Data = data;
        this.Type = data?.GetType();
        this.ObjectEncoder = JsonObjectEncoder.Default;
        this.DataTypeTranslator = Bam.Net.Data.DataTypeTranslator.Default;
    }

    protected object Data
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
    
    [JsonIgnore]
    [YamlIgnore]
    public virtual Type Type { get; init; }

    private TypeDescriptor _typeDescriptor;
    public TypeDescriptor TypeDescriptor
    {
        get
        {
            if (_typeDescriptor == null && Type != null)
            {
                _typeDescriptor = new TypeDescriptor(Type);
            }

            return _typeDescriptor;
        }
        set => _typeDescriptor = value;
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
            if (TypeDescriptor != null && TypeDescriptor.Type != null)
            {
                foreach (var objectProperty in GetObjectProperties()) yield return objectProperty;
            }
        }
        set => _properties = value;
    }

    private IEnumerable<IObjectProperty> GetObjectProperties()
    {
        foreach (PropertyInfo propertyInfo in TypeDescriptor.Type.GetProperties())
        {
            DataTypes enumType = DataTypeTranslator.EnumFromType(propertyInfo.PropertyType);
            if (enumType != DataTypes.Default)
            {
                yield return new ObjectProperty(propertyInfo, propertyInfo.GetValue(this.Data));
            }
        }
    }
}