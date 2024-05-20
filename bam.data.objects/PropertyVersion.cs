using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public class PropertyVersion : IPropertyVersion
{
    public PropertyVersion(int number = 1, string description = null)
    {
        this.Number = number;
        this.Description = description;
    }

    public PropertyVersion(IProperty property, int number = 1, string description = null)
    {
        this.Property = property;
        this.Number = number;
        this.Description = description;
    }

    public ITypeDescriptor TypeDescriptor => this.Property?.Parent?.Type;
    public IObjectData Parent => this.Property?.Parent;
    public IProperty Property { get; }
    public int Number { get; }
    public string Description { get; }
    public byte[]? Value { get; }
}