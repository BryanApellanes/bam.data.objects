using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyVersionHolder : IStorageHolder
{
    IPropertyHolder PropertyHolder { get; }
    string PropertyName { get; }
    IPropertyVersion PropertyVersion { get; }
    
    // {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat
    IPropertyStorageSlot Save(IProperty property); 
    IProperty Load();
}