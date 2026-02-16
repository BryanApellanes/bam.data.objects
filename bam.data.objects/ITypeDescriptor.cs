namespace Bam.Data.Objects;

/// <summary>
/// Describes a CLR type by its runtime <see cref="System.Type"/> reference and its assembly-qualified name string.
/// </summary>
public interface ITypeDescriptor
{
    /// <summary>
    /// Gets or sets the runtime type.
    /// </summary>
    Type Type { get; set; }

    /// <summary>
    /// Gets or sets the assembly-qualified type name used for serialization and type resolution.
    /// </summary>
    string AssemblyQualifiedTypeName { get; set; }
}