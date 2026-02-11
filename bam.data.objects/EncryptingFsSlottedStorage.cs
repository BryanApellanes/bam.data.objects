using Bam.Encryption;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class EncryptingFsSlottedStorage : FsSlottedStorage
{
    public EncryptingFsSlottedStorage(string path, IEncryptor encryptor, IDecryptor decryptor) : base(path)
    {
        this.Encryptor = encryptor;
        this.Decryptor = decryptor;
    }

    private IEncryptor Encryptor { get; }
    private IDecryptor Decryptor { get; }

    public override IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        byte[] encrypted = Encryptor.Encrypt(rawData.Value);
        IRawData encryptedRawData = new RawData(encrypted);
        return base.Save(slot, encryptedRawData);
    }

    public override IRawData LoadSlot(IStorageSlot slot)
    {
        IRawData encryptedRawData = base.LoadSlot(slot);
        byte[] decrypted = Decryptor.Decrypt(encryptedRawData.Value);
        return new RawData(decrypted);
    }
}
