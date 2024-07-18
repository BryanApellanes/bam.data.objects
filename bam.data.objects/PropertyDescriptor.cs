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
        this.PropertyType = property.Type;
    }
    public IObjectKey ObjectKey { get; set; }
    public string PropertyName { get; set; }
    public Type PropertyType { get; set; }
}