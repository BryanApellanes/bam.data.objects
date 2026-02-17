using System.Text;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a named property of an object, including its serialized value, type information, and storage path.
/// </summary>
public interface IProperty : IJsonable
{
    /// <summary>
    /// Gets or sets the parent object data that owns this property.
    /// </summary>
    IObjectData Parent { get; set; }

    /// <summary>
    /// Gets or sets the assembly-qualified type name of the parent object.
    /// </summary>
    string AssemblyQualifiedTypeName { get; set; }

    /// <summary>
    /// Gets or sets the name of this property.
    /// </summary>
    string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the CLR type of this property.
    /// </summary>
    Type Type { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized value of this property.
    /// </summary>
    string Value { get; set; }

    /// <summary>
    /// Gets the unversioned relative storage path for this property, combining namespace, type, key, and property name.
    /// </summary>
    string StorageSlotRelativePath { get; }

    /// <summary>
    /// Gets or sets the version history for this property.
    /// </summary>
    IEnumerable<IPropertyRevision> Versions { get; set; }

    /// <summary>
    /// Decodes the serialized value back into its original object form.
    /// </summary>
    /// <returns>The decoded property value.</returns>
    object Decode();

    /// <summary>
    /// Sets the decoded value of this property on the specified target object.
    /// </summary>
    /// <param name="target">The target object to set the value on.</param>
    /// <returns>The target object with the property value set.</returns>
    object SetValue(object target);

    /// <summary>
    /// Encodes the specified value, updates this property, and sets it on the target object.
    /// </summary>
    /// <param name="target">The target object to set the value on.</param>
    /// <param name="value">The new value to assign.</param>
    /// <returns>The target object with the property value set.</returns>
    object SetValue(object target, object value);

    /// <summary>
    /// Converts this property to raw data suitable for storage.
    /// </summary>
    /// <param name="encoding">The text encoding to use, or null for UTF-8.</param>
    /// <returns>The raw data representation of this property.</returns>
    IRawData ToRawData(Encoding encoding = null!);

    /// <summary>
    /// Converts this property to a raw data pointer (the hash hex string of the raw data).
    /// </summary>
    /// <param name="encoding">The text encoding to use, or null for UTF-8.</param>
    /// <returns>The raw data pointer for this property.</returns>
    IRawData ToRawDataPointer(Encoding encoding = null!);

    /// <summary>
    /// Creates a property descriptor from this property.
    /// </summary>
    /// <returns>A descriptor containing the object key, property name, and property type.</returns>
    IPropertyDescriptor ToDescriptor();
}