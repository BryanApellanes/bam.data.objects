using Bam.Data.Objects;

namespace Bam.Storage;

public interface IObjectStorageSaveResult 
{
    IObjectDataReader ObjectDataReader { get; set; }
}