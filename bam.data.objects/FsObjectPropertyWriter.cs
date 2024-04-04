using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Repositories;
using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class FsObjectPropertyWriter : IObjectPropertyWriter
{
    public FsObjectPropertyWriter(IObjectStorageManager objectStorageManager, IObjectHashCalculator objectHashCalculator, IObjectConverter objectConverter)
    {
        this.ObjectStorageManager = objectStorageManager;
        this.ObjectHashCalculator = objectHashCalculator;
        this.ObjectConverter = objectConverter;
    }
    
    public IObjectStorageManager ObjectStorageManager { get; private set; }
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
        /*object propertyValue = property.GetValue(parentDataObject);
        ObjectProperty toWrite = new ObjectProperty(property, propertyValue);
        DirectoryInfo directoryInfo = this.ObjectStorageManager.GetPropertyStorage(property.DeclaringType, property);

        string parentKey = ObjectHashCalculator.CalculateKeyHash(parentDataObject);
        string parentHash = ObjectHashCalculator.CalculateHash(parentDataObject);
        string toWriteString = GetStringData(toWrite);

        long version = GetCurrentVersion(ObjectStorageManager.GetKeyStorage(property));*/
        throw new NotImplementedException("this is not complete");
    }

    protected virtual string GetStringData(object propertyValue)
    {
        return ObjectConverter.Stringify(propertyValue);
    }
    
    protected DirectoryInfo GetNextVersionDirectory(PropertyInfo property)
    {
        IStorageIdentifier keyDirectory = this.ObjectStorageManager.GetKeyStorage(property.DeclaringType, property);
        return new DirectoryInfo(Path.Combine(keyDirectory.Value, GetNextVersion(new DirectoryInfo(keyDirectory.Value)).ToString()));
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