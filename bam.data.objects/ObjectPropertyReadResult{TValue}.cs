using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace bam.data.dynamic.Objects;

public class ObjectPropertyReadResult<TValue> : IObjectPropertyReadResult<TValue>
{
    public Type Type { get; }
    public PropertyInfo Property { get; }
    public TValue Value { get; }

    object IObjectPropertyReadResult.Value => Value;

    public bool Success { get; }
}