using System.Text;
using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectProperty
{
    IObjectData Parent { get; set; }
    string AssemblyQualifiedTypeName { get; set; }
    string PropertyName { get; set; }
    string Value { get; set; }

    object Decode();
    object SetValue(object target);
    object SetValue(object target, object value);
    IRawData ToRawData(Encoding encoding = null);
}