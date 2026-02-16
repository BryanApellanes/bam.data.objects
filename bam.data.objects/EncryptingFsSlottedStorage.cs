using Bam.Encryption;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Extends <see cref="FsSlottedStorage"/> to transparently encrypt data on save and decrypt on load.
/// </summary>
public class EncryptingFsSlottedStorage : FsSlottedStorage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptingFsSlottedStorage"/> class.
    /// </summary>
    /// <param name="path">The file system path for the slotted storage root.</param>
    /// <param name="encryptor">The encryptor used to encrypt raw data before writing.</param>
    /// <param name="decryptor">The decryptor used to decrypt raw data after reading.</param>
    public EncryptingFsSlottedStorage(string path, IEncryptor encryptor, IDecryptor decryptor) : base(path)
    {
        this.Encryptor = encryptor;
        this.Decryptor = decryptor;
    }

    private IEncryptor Encryptor { get; }
    private IDecryptor Decryptor { get; }

    /// <summary>
    /// Encrypts the raw data and saves it to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to save to.</param>
    /// <param name="rawData">The raw data to encrypt and save.</param>
    /// <returns>The storage slot where the encrypted data was saved.</returns>
    public override IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        byte[] encrypted = Encryptor.Encrypt(rawData.Value);
        IRawData encryptedRawData = new RawData(encrypted);
        return base.Save(slot, encryptedRawData);
    }

    /// <summary>
    /// Loads and decrypts the raw data from the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to load from.</param>
    /// <returns>The decrypted raw data.</returns>
    public override IRawData LoadSlot(IStorageSlot slot)
    {
        IRawData encryptedRawData = base.LoadSlot(slot);
        byte[] decrypted = Decryptor.Decrypt(encryptedRawData.Value);
        return new RawData(decrypted);
    }
}
