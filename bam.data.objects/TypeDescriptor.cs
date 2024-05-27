using System.Text.Json.Serialization;
using Bam;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

public class TypeDescriptor : ITypeDescriptor
{
    public static implicit operator Type(TypeDescriptor typeDescriptor)
    {
        return typeDescriptor.Type;
    }

    public static implicit operator string(TypeDescriptor typeDescriptor)
    {
        return typeDescriptor.AssemblyQualifiedTypeName;
    }

    public static implicit operator TypeDescriptor(Type type)
    {
        return new TypeDescriptor(type);
    }
    
    public TypeDescriptor(Type type)
    {
        this.Type = type;
    }

    public TypeDescriptor(string assemblyQualifiedTypeName)
    {
        this.AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
    }
    
    private Type _type;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type
    {
        get
        {
            if (_type == null && !string.IsNullOrEmpty(AssemblyQualifiedTypeName))
            {
                _type = Type.GetType(AssemblyQualifiedTypeName);
            }

            return _type;
        }
        set => _type = value;
    }

    private string _typeName;

    public string AssemblyQualifiedTypeName
    {
        get
        {
            if (string.IsNullOrEmpty(_typeName) && Type != null)
            {
                _typeName = Type.AssemblyQualifiedName;
            }

            return _typeName;
        }
        set => _typeName = value;
    }
}