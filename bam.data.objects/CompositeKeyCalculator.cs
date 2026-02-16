using System.Reflection;
using System.Text;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

/// <summary>
/// Calculates composite keys for objects by hashing properties adorned with <see cref="Bam.Data.Repositories.CompositeKeyAttribute"/>.
/// </summary>
public class CompositeKeyCalculator: ICompositeKeyCalculator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeKeyCalculator"/> class with SHA256 as the default hash algorithm.
    /// </summary>
    public CompositeKeyCalculator()
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
    }

    /// <summary>
    /// Gets or sets the hash algorithm used for key calculations.
    /// </summary>
    public HashAlgorithms HashAlgorithm { get; set; }

    /// <summary>
    /// Gets or sets the text encoding used when converting data to bytes for hashing.
    /// </summary>
    public Encoding Encoding { get; set; }

    /// <inheritdoc />
    public ulong CalculateULongKey(object instance)
    {
        if (instance == null)
        {
            return string.Empty.ToHashULong(this.HashAlgorithm, this.Encoding);
        }

        SortedDictionary<string, string> jsonify = GetJsonDictionary(instance);

        return jsonify.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }
    
    /// <inheritdoc />
    public ulong CalculateULongKey(IObjectData objectData)
    {
        if (objectData == null)
        {
            return string.Empty.ToHashULong(this.HashAlgorithm, this.Encoding);
        }
        
        SortedDictionary<string, string> jsonify = GetJsonDictionary(objectData);

        return jsonify.ToJson().ToHashULong(this.HashAlgorithm, this.Encoding);
    }

    /// <inheritdoc />
    public string CalculateHashHexKey(object instance)
    {
        if (instance == null)
        {
            return string.Empty.HashHexString(this.HashAlgorithm, this.Encoding);
        }

        SortedDictionary<string, string> jsonify = GetJsonDictionary(instance);

        return jsonify.ToJson().HashHexString(this.HashAlgorithm, this.Encoding);
    }

    /// <inheritdoc />
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