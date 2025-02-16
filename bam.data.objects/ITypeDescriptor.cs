namespace Bam.Data.Objects;

public interface ITypeDescriptor
{
    Type Type { get; set; }
    string AssemblyQualifiedTypeName { get; set; }
}