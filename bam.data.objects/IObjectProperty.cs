using System.Text;
using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectProperty
{
    /// <summary>
    /// Gets or sets the identifier for the object instance this property belongs to.
    /// </summary>
    //ulong ParentHashId { get; set; }
    IObjectData Data { get; set; }
    string AssemblyQualifiedTypeName { get; set; }
    string PropertyName { get; set; }
    string Value { get; set; }

    object Decode();
    object SetValue(object target);
    IRawData ToRawData(Encoding encoding = null);
}