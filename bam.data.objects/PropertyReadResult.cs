using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Repositories;

public class PropertyReadResult : IPropertyReadResult
{
    public Type Type { get; }
    public PropertyInfo Property { get; }
    public object Value { get; }
    public bool Success { get; }
}