namespace Bam.Data.Objects;

public class PropertyDescriptor : IPropertyDescriptor
{
    public PropertyDescriptor()
    {
    }
    
    public PropertyDescriptor(IProperty property)
    {
        Args.ThrowIfNull(property.Parent);
        this.ObjectKey = property.Parent.GetObjectKey();
        this.PropertyName = property.PropertyName;
    }
    public IObjectKey ObjectKey { get; set; }
    public string PropertyName { get; set; }
}