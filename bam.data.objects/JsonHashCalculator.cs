using System.Text;
using Bam.Data.Objects;
using Bam.Net;

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

    public ulong CalculateHash(object instance)
    {
        return CalculateHash(new ObjectData(instance));
    }
    
    public ulong CalculateHash(IObjectData data)
    {
        return data.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }
}