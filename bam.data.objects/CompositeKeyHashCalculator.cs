using System.Reflection;
using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam.Net;
using Bam.Net.Data.Repositories;

namespace Bam.Data.Objects;

public class CompositeKeyHashCalculator: IKeyHashCalculator
{
    public CompositeKeyHashCalculator()
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
    }
    public HashAlgorithms HashAlgorithm { get; set; }
    public Encoding Encoding { get; set; }
    public string CalculateKeyHash(object instance)
    {
        if (instance == null)
        {
            return string.Empty.HashHexString(this.HashAlgorithm, this.Encoding);
        }

        Type type = instance.GetType();
        Dictionary<string, string> jsonify = new Dictionary<string, string>
        {
            { "type", type.AssemblyQualifiedName }
        };
        this.AddCompositeKeys(type, instance, jsonify);
        
        return jsonify.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }

    protected void AddCompositeKeys(Type type, object instance, Dictionary<string, string> dictionary)
    {
        foreach (PropertyInfo property in type.GetProperties()
                     .Where(propertyInfo => propertyInfo.HasCustomAttributeOfType<CompositeKeyAttribute>()))
        {
            dictionary.Add(property.Name, property.GetValue(instance).ToJson());
        }
    }
}