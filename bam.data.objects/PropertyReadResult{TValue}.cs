using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace bam.data.dynamic.Objects;

public class PropertyReadResult<TValue> : IPropertyReadResult<TValue>
{
    public Type Type { get; }
    public PropertyInfo Property { get; }
    public TValue Value { get; }

    object IPropertyReadResult.Value => Value;

    public bool Success { get; }
}