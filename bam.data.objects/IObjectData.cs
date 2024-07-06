using System.Text.Json.Serialization;
using Bam.Data.Dynamic.Objects;
using Bam;
using Bam.Storage;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

public interface IObjectData : IJsonable
{
    [JsonIgnore]
    [YamlIgnore]
    object Data { get; set; }
    TypeDescriptor Type { get; }
    IObjectIdentifierFactory ObjectIdentifierFactory { get; set; }
    IProperty? Property(string propertyName);
    IObjectData? Property(string propertyName, object value);
    IEnumerable<IProperty> Properties { get; }
    
    IObjectEncoding Encode();

    IObjectKey GetObjectKey();
    IObjectIdentifier GetObjectIdentifier();
}