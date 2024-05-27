using System.Text;
using Bam.Data.Objects;
using Bam;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public interface IProperty : IJsonable
{
    IObjectData Parent { get; set; }
    string AssemblyQualifiedTypeName { get; set; }
    string PropertyName { get; set; }
    string Value { get; set; }
    string StorageSlotRelativePath { get; }

    object Decode();
    object SetValue(object target);
    object SetValue(object target, object value);
    IRawData ToRawData(Encoding encoding = null);
}