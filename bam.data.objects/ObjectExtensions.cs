using System.Reflection;
using Bam.Data.Objects;
using Bam.Net;

namespace Bam.Data.Dynamic.Objects;

public static class ObjectExtensions
{
    
    public static IEnumerable<IObjectProperty> ToObjectProperties(this object data)
    {
        Args.ThrowIfNull(data, nameof(data));
        Type type = data.GetType();
        return new ObjectData(data).Properties;
    }
}