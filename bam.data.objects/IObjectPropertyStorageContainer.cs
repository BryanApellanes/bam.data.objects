using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectPropertyStorageContainer : IStorageContainer
{
    IVersion Version { get; }
    IList<IVersion> VersionHistory { get; }
    IVersion NextVersion { get; }

    IObjectPropertyWriteResult Save(IObjectStorageManager storageManager, IObjectProperty objectProperty);
}