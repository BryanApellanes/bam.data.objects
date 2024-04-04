using Bam.Data.Objects;

namespace Bam.Storage;

public interface IObjectStorageSaveResult 
{
    IObjectIdentifier ObjectIdentifier { get; set; }
}