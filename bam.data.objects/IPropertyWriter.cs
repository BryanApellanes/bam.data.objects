using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;

namespace Bam.Data.Repositories;

public interface IPropertyWriter
{
    Task<IPropertyWriteResult> WritePropertyAsync(IProperty property);
}