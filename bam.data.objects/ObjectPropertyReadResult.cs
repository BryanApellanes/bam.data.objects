using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Net.Data.Repositories;

public class ObjectPropertyReadResult : IObjectPropertyReadResult
{
    public Type Type { get; }
    public PropertyInfo Property { get; }
    public object Value { get; }
    public bool Success { get; }
}