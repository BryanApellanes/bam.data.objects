using System.Text;
using Bam;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IProperty : IJsonable
{
    IObjectData Parent { get; set; }
    string AssemblyQualifiedTypeName { get; set; }
    string PropertyName { get; set; }
    Type Type { get; set; }
    string Value { get; set; }
    string StorageSlotRelativePath { get; }

    IEnumerable<IPropertyVersion> Versions { get; set; }
    
    object Decode();
    object SetValue(object target);
    object SetValue(object target, object value);
    IRawData ToRawData(Encoding encoding = null);
    IRawData ToRawDataPointer(Encoding encoding = null);

    IPropertyDescriptor ToDescriptor();
}