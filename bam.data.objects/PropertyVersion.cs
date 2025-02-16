namespace Bam.Data.Objects;

public class PropertyVersion : IPropertyVersion
{
    public PropertyVersion(IProperty property, int number = 1, string metaData = null)
    {
        this.Property = property;
        this.Number = number;
        this.MetaData = metaData;
    }

    public ITypeDescriptor TypeDescriptor => this.Property?.Parent?.TypeDescriptor;
    public IObjectData Parent => this.Property?.Parent;
    public IProperty Property { get; }
    public int Number { get; }
    public string MetaData { get; }
}