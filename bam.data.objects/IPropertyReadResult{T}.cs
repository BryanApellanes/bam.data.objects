using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

public interface IPropertyReadResult<TValue> : IPropertyReadResult
{
    Type Type { get; }
    PropertyInfo Property { get; }
    new TValue Value { get; }
    bool Success { get; }
}