using System.Text;
using Bam.Data.Objects;
using Bam;

namespace Bam.Data.Dynamic.Objects;

public class JsonHashCalculator : IHashCalculator
{
    public JsonHashCalculator()
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
        this.Encoding = Encoding.UTF8;
    }
    
    public HashAlgorithms HashAlgorithm { get; set; }
    public Encoding Encoding { get; set; }

    public ulong CalculateULongHash(object instance)
    {
        if (instance is IObjectData data)
        {
            return CalculateULongHash(data);
        }
        return CalculateULongHash(new ObjectData(instance));
    }
    
    public ulong CalculateULongHash(IObjectData data)
    {
        return data.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }

    public string CalculateHashHex(object data)
    {
        if (data is IObjectData objectData)
        {
            return CalculateHashHex(data);
        }

        return CalculateHashHex(new ObjectData(data));
    }

    public string CalculateHashHex(IObjectData data)
    {
        return data.ToJson().HashHexString(HashAlgorithm);
    }
}