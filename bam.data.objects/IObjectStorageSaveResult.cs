using Bam.Data.Objects;

namespace Bam.Storage;

public interface IObjectStorageSaveResult 
{
    IObjectLoader ObjectLoader { get; set; }
}