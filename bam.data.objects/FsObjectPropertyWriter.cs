using System.Reflection;
using Bam.Data.Repositories;
using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class FsObjectPropertyWriter : IObjectPropertyWriter
{
    public FsObjectPropertyWriter(IObjectFs objectFs, IObjectHashCalculator objectHashCalculator, IObjectConverter objectConverter)
    {
        this.ObjectFs = objectFs;
        this.ObjectHashCalculator = objectHashCalculator;
        this.ObjectConverter = objectConverter;
    }
    
    public IObjectFs ObjectFs { get; private set; }
    public IObjectHashCalculator ObjectHashCalculator { get; private set; }
    
    public IObjectConverter ObjectConverter { get; private set; }

    public Task<IObjectPropertyWriteResult> WritePropertyAsync(PropertyInfo property, object parentDataObject)
    {
        return WritePropertyAsync(property, property.GetValue(parentDataObject), parentDataObject);
    }

    public async Task<IObjectPropertyWriteResult> WritePropertyAsync(PropertyInfo property, object propertyValue, object parentDataObject)
    {   
        ObjectPropertyWriteResult result = new ObjectPropertyWriteResult
        {
            ObjectProperty = new ObjectProperty(property, propertyValue)
            {
                ParentHash = ObjectHashCalculator.CalculateHash(parentDataObject)
            },
            Success = true
        };

        try
        {
            CommitProperty(property, parentDataObject);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.GetMessageAndStackTrace();
        }

        return result;
    }

    protected virtual void CommitProperty(PropertyInfo property, object parentDataObject)
    {
        object propertyValue = property.GetValue(parentDataObject);
        ObjectProperty toWrite = new ObjectProperty(property, propertyValue);
        DirectoryInfo directoryInfo = this.ObjectFs.GetPropertyDirectory(property.DeclaringType, property);

        string parentKey = ObjectHashCalculator.CalculateKeyHash(parentDataObject);
        string parentHash = ObjectHashCalculator.CalculateHash(parentDataObject);
        string toWriteString = GetStringData(toWrite);

        long version = GetCurrentVersion(ObjectFs.GetKeysDirectory(property));
        throw new NotImplementedException("this is not complete");
    }

    protected virtual string GetStringData(object propertyValue)
    {
        return ObjectConverter.Stringify(propertyValue);
    }
    
    protected DirectoryInfo GetNextVersionDirectory(PropertyInfo property)
    {
        DirectoryInfo keyDirectory = this.ObjectFs.GetKeysDirectory(property.DeclaringType, property);
        return new DirectoryInfo(Path.Combine(keyDirectory.FullName, GetNextVersion(keyDirectory).ToString()));
    }

    public virtual long GetCurrentVersion(DirectoryInfo keyDirectory)
    {
        List<long> versions = new List<long>();
        foreach (DirectoryInfo subDir in keyDirectory.GetDirectories())
        {
            if (long.TryParse(subDir.Name, out long num))
            {
                versions.Add(num);
            }
        }

        return versions.ToArray().Largest();
    }
    
    protected virtual int GetNextVersion(DirectoryInfo keyDirectory)
    {
        int num = 1;
        while (Directory.Exists(Path.Combine(keyDirectory.FullName, num.ToString())))
        {
            num++;
        }

        return num;
    }
}