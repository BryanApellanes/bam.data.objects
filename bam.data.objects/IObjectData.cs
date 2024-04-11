using System.Text.Json.Serialization;
using Bam.Data.Dynamic.Objects;
using Bam.Net;
using Bam.Storage;
using YamlDotNet.Serialization;

namespace Bam.Data.Objects;

public interface IObjectData 
{
    [JsonIgnore]
    [YamlIgnore]
    object Data { get; set; }
    TypeDescriptor Type { get; }
    IObjectProperty? Property(string propertyName);
    IObjectData? Property(string propertyName, object value);
    IEnumerable<IObjectProperty> Properties { get; }
    string ToJson();
    IObjectEncoding Encode();
    ulong GetHash(IHashCalculator hashCalculator);
    ulong GetKeyHash(IKeyHashCalculator keyHashCalculator);
}