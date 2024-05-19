using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageHolder : IStorageHolder
{
    ITypeStorageHolder TypeStorageHolder { get; }
    IVersion Version { get; }
    IList<IVersion> VersionHistory { get; }
    IVersion NextVersion { get; }

    IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property);
}