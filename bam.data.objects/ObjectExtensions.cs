using System.Reflection;
using Bam.Data.Objects;
using Bam;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public static class ObjectExtensions
{
    
    public static IEnumerable<IProperty> ToObjectProperties(this object data)
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