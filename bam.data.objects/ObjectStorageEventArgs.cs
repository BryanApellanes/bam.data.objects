using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Storage;

namespace Bamn.Data.Objects;

public class ObjectStorageEventArgs : EventArgs
{
    public IPropertyWriteResult PropertyWriteResult { get; set; }
    public Exception Exception { get; set; }
    public IPropertyDescriptor PropertyDescriptor { get; set; }
    public IStorageSlot ReadingFrom { get; set; } 
}