using System.Reflection;
using Bam.Data.Dynamic.Objects;

namespace Bam.Net.Data.Repositories;

public interface IObjectPropertyWriter
{
    Task<IObjectPropertyWriteResult> WritePropertyAsync(IObjectProperty property);
}