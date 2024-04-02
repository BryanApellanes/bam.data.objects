using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Net.Data.Repositories;

public interface IObjectPropertyWriter
{
    Task<IObjectPropertyWriteResult> WritePropertyAsync(PropertyInfo property, object propertyValue);
    
    Task<IObjectPropertyWriteResult> WritePropertyAsync(PropertyInfo property, object propertyValue, object parentDataObject);
}