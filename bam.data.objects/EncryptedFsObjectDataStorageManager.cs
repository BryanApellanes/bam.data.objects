using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public class EncryptedFsObjectDataStorageManager : FsObjectDataStorageManager
{
    public EncryptedFsObjectDataStorageManager(IRootStorageHolder rootStorage, IObjectDataFactory objectDataFactory, IEncryptor encryptor, IDecryptor decryptor)
        : base(rootStorage, objectDataFactory)
    {
        this.Encryptor = encryptor;
        this.Decryptor = decryptor;
    }

    private IEncryptor Encryptor { get; }
    private IDecryptor Decryptor { get; }

    public override IRawStorage GetRawStorage()
    {
        return new EncryptingFsSlottedStorage(Path.Combine(GetRootStorageHolder().FullName, "raw"), Encryptor, Decryptor);
    }
}
