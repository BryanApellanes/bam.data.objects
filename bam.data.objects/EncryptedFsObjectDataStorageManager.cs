using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Extends <see cref="FsObjectDataStorageManager"/> to encrypt raw data at rest using the provided encryptor and decryptor.
/// </summary>
public class EncryptedFsObjectDataStorageManager : FsObjectDataStorageManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedFsObjectDataStorageManager"/> class.
    /// </summary>
    /// <param name="rootStorage">The root storage holder for the file system storage location.</param>
    /// <param name="objectDataFactory">The factory used to create object data wrappers.</param>
    /// <param name="encryptor">The encryptor used to encrypt raw data before writing.</param>
    /// <param name="decryptor">The decryptor used to decrypt raw data after reading.</param>
    public EncryptedFsObjectDataStorageManager(IRootStorageHolder rootStorage, IObjectDataFactory objectDataFactory, IEncryptor encryptor, IDecryptor decryptor)
        : base(rootStorage, objectDataFactory)
    {
        this.Encryptor = encryptor;
        this.Decryptor = decryptor;
    }

    private IEncryptor Encryptor { get; }
    private IDecryptor Decryptor { get; }

    /// <summary>
    /// Gets the raw storage, returning an encrypting variant that transparently encrypts and decrypts data.
    /// </summary>
    /// <returns>An <see cref="EncryptingFsSlottedStorage"/> instance that encrypts data on save and decrypts on load.</returns>
    public override IRawStorage GetRawStorage()
    {
        return new EncryptingFsSlottedStorage(Path.Combine(GetRootStorageHolder().FullName, "raw"), Encryptor, Decryptor);
    }
}
