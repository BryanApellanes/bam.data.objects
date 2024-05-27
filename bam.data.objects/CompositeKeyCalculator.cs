using System.Reflection;
using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam;
using Bam.Data.Repositories;
using MongoDB.Bson;

namespace Bam.Data.Objects;

public class CompositeKeyCalculator: IKeyCalculator
{
    public CompositeKeyCalculator()
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
    }
    public HashAlgorithms HashAlgorithm { get; set; }
    public Encoding Encoding { get; set; }
    public ulong CalculateULongKey(object instance)
    {
        if (instance == null)
        {
            return string.Empty.ToHashULong(this.HashAlgorithm, this.Encoding);
        }

        Dictionary<string, string> jsonify = GetJsonDictionary(instance);

        return jsonify.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }
    
    public ulong CalculateULongKey(IObjectData objectData)
    {
        if (objectData == null)
        {
            return string.Empty.ToHashULong(this.HashAlgorithm, this.Encoding);
        }
        
        Dictionary<string, string> jsonify = GetJsonDictionary(objectData);

        return jsonify.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }

    public string CalculateHashHexKey(object instance)
    {
        if (instance == null)
        {
            return string.Empty.HashHexString(this.HashAlgorithm, this.Encoding);
        }

        Dictionary<string, string> jsonify = GetJsonDictionary(instance);

        return jsonify.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }

    public string CalculateHashHexKey(IObjectData objectData)
    {
        if (objectData == null)
        {
            return string.Empty.HashHexString(this.HashAlgorithm, this.Encoding);
        }
        
        Dictionary<string, string> jsonify = GetJsonDictionary(objectData);

        return jsonify.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }
    
    private Dictionary<string, string> GetJsonDictionary(IObjectData objectData)
    {
        return GetJsonDictionary(objectData.Data);
    }
    
    private Dictionary<string, string> GetJsonDictionary(object instance)
    {
        Type type = instance.GetType();
        Dictionary<string, string> jsonify = new Dictionary<string, string>
        {
            { "type", type.AssemblyQualifiedName }
        };
        this.AddCompositeKeysToDictionary(type, instance, jsonify);
        return jsonify;
    }
    
    private void AddCompositeKeysToDictionary(Type type, object instance, Dictionary<string, string> dictionary)
    {
        foreach (PropertyInfo property in type.GetProperties()
                     .Where(propertyInfo => propertyInfo.HasCustomAttributeOfType<CompositeKeyAttribute>()))
        {
            dictionary.Add(property.Name, property.GetValue(instance).ToJson());
        }
    }
}