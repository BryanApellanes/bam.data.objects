using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Storage;

namespace Ban.Data.Objects;

public class ObjectPropertyStorageSlot : IStorageSlot
{
    public ObjectPropertyStorageSlot(IObjectProperty property, string path)
    {
        this.ObjectProperty = property;
        this.FullName = path;
    }
    public IObjectProperty ObjectProperty { get; init; }
    public string? FullName { get; }

    private IStorageContainer _storageContainer;
    public IStorageContainer StorageContainer =>
        _storageContainer ?? (_storageContainer =
            new ObjectPropertyStorageContainer(Path.GetDirectoryName(this.FullName)));

    public string Name => Path.GetFileName(this.FullName);
    public IStorageSlot Save(IStorage storage, IRawData rawData)
    {
        throw new NotImplementedException();
    }
}