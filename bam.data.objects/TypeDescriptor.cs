using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="ITypeDescriptor"/> that describes a CLR type by its runtime reference and assembly-qualified name, with implicit conversions.
/// </summary>
public class TypeDescriptor : ITypeDescriptor
{
    /// <summary>
    /// Implicitly converts a <see cref="TypeDescriptor"/> to its underlying <see cref="System.Type"/>.
    /// </summary>
    /// <param name="typeDescriptor">The type descriptor to convert.</param>
    public static implicit operator Type(TypeDescriptor typeDescriptor)
    {
        return typeDescriptor.Type;
    }

    /// <summary>
    /// Implicitly converts a <see cref="TypeDescriptor"/> to its assembly-qualified type name string.
    /// </summary>
    /// <param name="typeDescriptor">The type descriptor to convert.</param>
    public static implicit operator string(TypeDescriptor typeDescriptor)
    {
        return typeDescriptor.AssemblyQualifiedTypeName;
    }

    /// <summary>
    /// Implicitly converts a <see cref="System.Type"/> to a <see cref="TypeDescriptor"/>.
    /// </summary>
    /// <param name="type">The type to wrap.</param>
    public static implicit operator TypeDescriptor(Type type)
    {
        return new TypeDescriptor(type);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDescriptor"/> class from the specified runtime type.
    /// </summary>
    /// <param name="type">The runtime type to describe.</param>
    public TypeDescriptor(Type type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDescriptor"/> class from the specified assembly-qualified type name.
    /// </summary>
    /// <param name="assemblyQualifiedTypeName">The assembly-qualified name of the type.</param>
    public TypeDescriptor(string assemblyQualifiedTypeName)
    {
        this.AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
    }
    
    private Type _type;

    /// <inheritdoc />
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

    /// <inheritdoc />
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