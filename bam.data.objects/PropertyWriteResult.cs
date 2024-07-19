using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;



public class PropertyWriteResult : IPropertyWriteResult
{
    public IObjectDataKey ObjectDataKey { get; set; }
    public IStorageSlot PointerStorageSlot { get; set; }
    public IStorageSlot ValueStorageSlot { get; set; }
    public IProperty Property { get; set; }
    public IRawData RawData { get; set; }
    public PropertyWriteResults Status { get; set; }
    public string Message { get; set; }
    public string RawDataHash { get; set; }
    public IPropertyDescriptor GetDescriptor()
    {
        return new PropertyDescriptor()
        {
            ObjectDataKey = this.ObjectDataKey,
            PropertyName = Property.PropertyName
        };
    }
}