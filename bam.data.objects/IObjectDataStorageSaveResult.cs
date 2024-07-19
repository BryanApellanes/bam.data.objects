using Bam.Data.Objects;

namespace Bam.Storage;

public interface IObjectDataStorageSaveResult 
{
    IObjectDataReader ObjectDataReader { get; set; }
}