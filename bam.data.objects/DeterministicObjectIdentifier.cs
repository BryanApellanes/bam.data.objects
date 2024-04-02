using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Dynamic;

public class DeterministicObjectIdentifier
{
    public Type Type { get; set; }
    public string Hash { get; set; }
    public string KeyHash { get; set; }
}