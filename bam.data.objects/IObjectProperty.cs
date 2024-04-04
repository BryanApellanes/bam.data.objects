using System.Text;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectProperty
{
    string AssemblyQualifiedTypeName { get; set; }
    string PropertyName { get; set; }
    string Value { get; set; }

    object Decode();
    object SetValue(object target);
    IRawData ToRawData(Encoding encoding = null);
}