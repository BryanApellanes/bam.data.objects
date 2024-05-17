using Bam.Data.Dynamic.Objects;
using Bam.Storage;
using Bam.Net;

namespace Bam.Data.Objects;

public class ObjectPropertyStorageContainer : DirectoryStorageContainer, IObjectPropertyStorageContainer
{
    public ObjectPropertyStorageContainer(string path) : base(path)
    {
    }

    public ObjectPropertyStorageContainer(DirectoryInfo directory) : base(directory)
    {
    }

    public override string? FullName => Path.Combine(base.FullName, GetNextVersion().ToString());
    
    private IVersion _version;

    public IVersion Version
    {
        get { return _version ??= new Version(GetNextVersion()); }
    }

    private IList<IVersion> _versionHistory;

    public IList<IVersion> VersionHistory
    {
        get { return _versionHistory ??= GetVersionHistory(); }
    }

    public IVersion NextVersion => new Version(Version.Number);

    public IObjectPropertyWriteResult Save(IObjectStorageManager storageManager, IObjectProperty objectProperty)
    {
        try
        {
            // write Object properties to
            // {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat content -> {RawDataHash}
            
            IStorage propertyStorage = storageManager.GetStorage(this);
            IRawData rawData = objectProperty.ToRawData();
            propertyStorage.Save(rawData);
            
            return new ObjectPropertyWriteResult()
            {
                Success = true,
                ObjectProperty = objectProperty,
                RawData = rawData,
                StorageSlot = propertyStorage.Save(objectProperty.ToRawData())
            };
        }
        catch (Exception ex)
        {
            return new ObjectPropertyWriteResult()
            {
                Success = false,
                ObjectProperty = objectProperty,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace()
            };
        }
    }
    
    private int GetNextVersion()
    {
        int number = 1;
        while (System.IO.Directory.Exists(Path.Combine(base.FullName, number.ToString())))
        {
            number++;
        }
        return number;
    }

    private IList<IVersion> GetVersionHistory()
    {
        DirectoryInfo root = new DirectoryInfo(base.FullName);
        List<IVersion> versions = new List<IVersion>();
        foreach (DirectoryInfo subDirectory in root.GetDirectories())
        {
            if (int.TryParse(subDirectory.Name, out int version))
            {
                string descriptionFile = Path.Combine(subDirectory.FullName, "desc");
                string description = File.Exists(descriptionFile) ? File.ReadAllText(descriptionFile) : string.Empty;
                
                string datFile = Path.Combine(subDirectory.FullName, "dat");
                byte[]? data = File.Exists(datFile) ? File.ReadAllBytes(datFile) : null;
                versions.Add(new Version(data, version, description));
            }
        }

        return versions;
    }
}