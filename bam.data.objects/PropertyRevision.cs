namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IPropertyRevision"/> representing a specific version of a property value.
/// </summary>
public class PropertyRevision : IPropertyRevision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRevision"/> class.
    /// </summary>
    /// <param name="property">The property associated with this revision.</param>
    /// <param name="number">The 1-based revision number.</param>
    /// <param name="metaData">Optional metadata associated with this revision.</param>
    public PropertyRevision(IProperty property, int number = 1, string metaData = null)
    {
        this.Property = property;
        this.Number = number;
        this.MetaData = metaData;
    }

    /// <inheritdoc />
    public ITypeDescriptor TypeDescriptor => this.Property?.Parent?.TypeDescriptor;

    /// <inheritdoc />
    public IObjectData Parent => this.Property?.Parent;

    /// <inheritdoc />
    public IProperty Property { get; }

    /// <inheritdoc />
    public int Number { get; }

    /// <inheritdoc />
    public string MetaData { get; }
}