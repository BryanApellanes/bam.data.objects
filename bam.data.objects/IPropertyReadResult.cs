using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

public interface IPropertyReadResult
{
    Type Type { get; }
    PropertyInfo Property { get; }
    object Value { get; }
    bool Success { get; }
}