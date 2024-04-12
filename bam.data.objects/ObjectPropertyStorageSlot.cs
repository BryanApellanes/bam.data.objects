using Bam.Data.Dynamic.Objects;
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
}