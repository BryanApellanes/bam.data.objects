using System.Text;
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
    
    public string CalculateHash(object instance)
    {
        return instance.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }
}