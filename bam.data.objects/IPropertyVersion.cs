using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public interface IPropertyVersion
{
    ITypeDescriptor? TypeDescriptor { get; }
    IObjectData? Parent { get; }
    IProperty Property { get; }
    int Number { get; }
    string MetaData { get; }
}