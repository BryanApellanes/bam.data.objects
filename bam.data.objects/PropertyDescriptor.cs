namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IPropertyDescriptor"/>, describing a property by name, type, and associated object key.
/// </summary>
public class PropertyDescriptor : IPropertyDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
    /// </summary>
    public PropertyDescriptor()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDescriptor"/> class from the specified property, extracting the object key, name, and type.
    /// </summary>
    /// <param name="property">The property to create a descriptor for. Its Parent must not be null.</param>
    public PropertyDescriptor(IProperty property)
    {
        Args.ThrowIfNull(property.Parent);
        this.ObjectDataKey = property.Parent.GetObjectKey();
        this.PropertyName = property.PropertyName;
        this.PropertyType = property.Type;
    }
    /// <inheritdoc />
    public IObjectDataKey ObjectDataKey { get; set; }

    /// <inheritdoc />
    public string PropertyName { get; set; }

    /// <inheritdoc />
    public Type PropertyType { get; set; }
}