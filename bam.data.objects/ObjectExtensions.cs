using System.Reflection;
using Bam.Data.Objects;
using Bam.Net;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public static class ObjectExtensions
{
    
    public static IEnumerable<IObjectProperty> ToObjectProperties(this object data)
    {
        Args.ThrowIfNull(data, nameof(data));
        if (data is ObjectData objectData)
        {
            return objectData.Properties;
        }

        if (data is IObjectData iObjectData)
        {
            return iObjectData.Properties;
        }
        return new ObjectData(data).Properties;
    }
}