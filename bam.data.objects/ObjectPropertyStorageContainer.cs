using Bam.Data.Dynamic.Objects;
using Bam.Storage;
using Bam.Net;
using Ban.Data.Objects;

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

    public IObjectPropertyWriteResult Save(IObjectProperty objectProperty, IObjectStorageManager storageManager)
    {
        try
        {
            IStorage referenceStorage = storageManager.GetStorage(this);
            IRawData objectPropertyRawData = objectProperty.ToRawData();
            RawDataReference reference = new RawDataReference(objectPropertyRawData.HashString);
            string referenceDatFilePath = Path.Combine(objectPropertyRawData.HashString.Split(2).ToArray());
            referenceDatFilePath = Path.Combine(referenceDatFilePath, "dat");
            IRawData referenceDatFileRawData = referenceStorage.Save(referenceDatFilePath, reference);
            
            IStorage writeStorage = storageManager.GetRawStorage();
            IRawData savedPropertyRawData = writeStorage.Save(objectPropertyRawData);
            return new ObjectPropertyWriteResult()
            {
                Success = true,
                ObjectProperty = objectProperty,
                RawData = objectPropertyRawData,
                StorageIdentifier = new ObjectPropertyStorageSlot(objectProperty, Path.Combine(writeStorage.Identifier.FullName, objectPropertyRawData.HashId.ToString()))
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
                versions.Add(new Version(version, description));
            }
        }

        return versions;
    }
}