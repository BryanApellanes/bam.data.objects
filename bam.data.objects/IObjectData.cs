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
    TypeDescriptor TypeDescriptor { get; }
    IObjectDataLocatorFactory ObjectDataLocatorFactory { get; set; }
    IProperty? Property(string propertyName);
    IObjectData? Property(string propertyName, object value);
    IEnumerable<IProperty> Properties { get; }
    
    IObjectEncoding Encode();

    IObjectDataKey GetObjectKey();
    IObjectDataIdentifier GetObjectIdentifier();
}