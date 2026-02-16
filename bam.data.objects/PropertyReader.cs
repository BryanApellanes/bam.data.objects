using System.Reflection;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Default implementation of <see cref="IPropertyReader"/>. Not yet implemented.
/// </summary>
public class PropertyReader : IPropertyReader
{
    /// <inheritdoc />
    public IPropertyReadResult ReadProperty(Type type, PropertyInfo property)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPropertyReadResult<TValue> ReadProperty<TValue>(Type type, PropertyInfo property)
    {
        throw new NotImplementedException();
    }
}