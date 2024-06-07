using Bam.Data.Dynamic.Objects;

namespace Bamn.Data.Objects;

public class ObjectStorageEventArgs : EventArgs
{
    public IPropertyWriteResult PropertyWriteResult { get; set; }
    public Exception Exception { get; set; }
}