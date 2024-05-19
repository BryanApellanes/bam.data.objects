using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Net.Data.Repositories;

public interface IPropertyWriter
{
    Task<IPropertyWriteResult> WritePropertyAsync(IProperty property);
}