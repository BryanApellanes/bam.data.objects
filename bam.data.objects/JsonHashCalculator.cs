using System.Text;
using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Calculates hash values for objects by serializing them to JSON and computing a hash of the JSON representation.
/// </summary>
public class JsonHashCalculator : IHashCalculator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonHashCalculator"/> class with SHA256 and UTF-8 defaults.
    /// </summary>
    public JsonHashCalculator()
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
        this.Encoding = Encoding.UTF8;
    }

    /// <summary>
    /// Gets or sets the hash algorithm used for hash calculations.
    /// </summary>
    public HashAlgorithms HashAlgorithm { get; set; }

    /// <summary>
    /// Gets or sets the text encoding used when converting JSON to bytes for hashing.
    /// </summary>
    public Encoding Encoding { get; set; }

    /// <inheritdoc />
    public ulong CalculateULongHash(object instance)
    {
        if (instance is IObjectData data)
        {
            return CalculateULongHash(data);
        }
        return CalculateULongHash(new ObjectData(instance));
    }
    
    /// <inheritdoc />
    public ulong CalculateULongHash(IObjectData data)
    {
        return data.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }

    /// <inheritdoc />
    public string CalculateHashHex(object data)
    {
        if (data is IObjectData objectData)
        {
            return CalculateHashHex(data);
        }

        return CalculateHashHex(new ObjectData(data));
    }

    /// <inheritdoc />
    public string CalculateHashHex(IObjectData data)
    {
        return data.ToJson().HashHexString(HashAlgorithm);
    }
}