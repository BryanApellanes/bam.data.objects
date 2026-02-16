namespace Bam.Data.Objects;

/// <summary>
/// Describes a property by name, type, and its associated object key, used to locate property data in storage.
/// </summary>
public interface IPropertyDescriptor
{
    /// <summary>
    /// Gets or sets the object key that this property belongs to.
    /// </summary>
    IObjectDataKey ObjectDataKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the property.
    /// </summary>
    string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the CLR type of the property.
    /// </summary>
    Type PropertyType { get; set; }
}