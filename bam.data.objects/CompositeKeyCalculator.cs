using System.Reflection;
using System.Text;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

public class CompositeKeyCalculator: ICompositeKeyCalculator
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

        SortedDictionary<string, string> jsonify = GetJsonDictionary(instance);

        return jsonify.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }
    
    public ulong CalculateULongKey(IObjectData objectData)
    {
        if (objectData == null)
        {
            return string.Empty.ToHashULong(this.HashAlgorithm, this.Encoding);
        }
        
        SortedDictionary<string, string> jsonify = GetJsonDictionary(objectData);

        return jsonify.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }

    public string CalculateHashHexKey(object instance)
    {
        if (instance == null)
        {
            return string.Empty.HashHexString(this.HashAlgorithm, this.Encoding);
        }

        SortedDictionary<string, string> jsonify = GetJsonDictionary(instance);

        return jsonify.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }

    public string CalculateHashHexKey(IObjectData objectData)
    {
        if (objectData == null)
        {
            return string.Empty.HashHexString(this.HashAlgorithm, this.Encoding);
        }
        
        SortedDictionary<string, string> jsonify = GetJsonDictionary(objectData);

        return CalculatHashHexKey(jsonify);
    }

    private string CalculatHashHexKey(SortedDictionary<string, string> jsonify)
    {
        return jsonify.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }

    private SortedDictionary<string, string> GetJsonDictionary(IObjectData objectData)
    {
        return GetJsonDictionary(objectData.Data);
    }
    
    private SortedDictionary<string, string> GetJsonDictionary(object instance)
    {
        Type type = instance.GetType();
        SortedDictionary<string, string> jsonify = new SortedDictionary<string, string>
        {
            { "type", type.AssemblyQualifiedName }
        };
        this.AddCompositeKeysToDictionary(type, instance, jsonify);
        return jsonify;
    }
    
    private void AddCompositeKeysToDictionary(Type type, object instance, SortedDictionary<string, string> dictionary)
    {
        foreach (PropertyInfo property in type.GetProperties()
                     .Where(propertyInfo => propertyInfo.HasCustomAttributeOfType<CompositeKeyAttribute>()))
        {
            object? value = property.GetValue(instance);
            dictionary.Add(property.Name, value == null ? "null": value.ToJson());
        }
    }
}