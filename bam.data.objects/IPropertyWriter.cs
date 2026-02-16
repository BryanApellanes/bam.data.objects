using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;

namespace Bam.Data.Repositories;

/// <summary>
/// Defines the operation for writing a single property to storage.
/// </summary>
public interface IPropertyWriter
{
    /// <summary>
    /// Writes the specified property to storage asynchronously.
    /// </summary>
    /// <param name="property">The property to write.</param>
    /// <returns>The result of the write operation.</returns>
    Task<IPropertyWriteResult> WritePropertyAsync(IProperty property);
}