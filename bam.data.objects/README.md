# bam.data.objects

File-system-backed object data storage with content-addressed property values, revision tracking, and search indexing.

## Storage Architecture

### Pointer Slots (Revision System)

Each object property is stored as a series of **revision slots**. A revision slot is a small file containing a SHA256 hash hex string that points to the actual property value in the raw content-addressed store.

```
<root>/objects/<TypePath>/<KeySegment1>/<KeySegment2>/<PropertyName>/1  → pointer (hash hex string)
<root>/objects/<TypePath>/<KeySegment1>/<KeySegment2>/<PropertyName>/2  → pointer (revision 2)
```

Revision numbers are sequential integers starting at 1. The latest revision is determined by scanning for the highest numbered slot file that exists on disk.

### Raw Content-Addressed Store

Property values are serialized (typically as JSON) and stored in a content-addressed store under `<root>/raw/`. The storage path is derived by segmenting the SHA256 hash hex string of the serialized value:

```
<root>/raw/<segment1>/<segment2>/dat
```

This means identical property values share a single storage entry (deduplication).

### Indexing

- **Primary index**: Objects are keyed by a composite key derived from the object's `Id` property.
- **UUID index**: Objects are also indexed by their `Uuid` property for lookup by UUID.
- **Search index**: `ObjectDataSearchIndexer` creates index files mapping `SHA256(propertyValue)` → object keys. This enables search-by-property-value without scanning all objects.

### Deleting

When an object is deleted the following file is written:
- A file containing the time of deletion and the user who deleted the entry:
  - `{root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/deleted`

### Archiving

When an object is archived all deleted objects are moved to the archive folder maintaining their original relative path:
- `{root}/archive/name/space/type/{Ob/je/ct/Ke/y_}/*`

### Loading

To load an object by its key the property values are read from the dat files with the latest version:
- `{root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat` - contains the hash of the raw data which holds the property value

### Searching

When objects are searched an `ObjectDataSearch` is used to provide search criteria. The `ObjectDataSearcher` encodes search values using SHA256, looks up matching object keys in the search index, and retrieves matching objects via the storage manager.

## Encrypted (Opaque) Storage

`EncryptedFsObjectDataStorageManager` provides transparent encryption of property values at rest. The design encrypts only the raw content-addressed store, leaving pointer data and search indexes untouched.

### What Gets Encrypted

| Component | Encrypted? | Reason |
|---|---|---|
| Raw property values (`<root>/raw/`) | Yes | Contains actual data (JSON-serialized values) |
| Pointer slots (revision files) | No | Already opaque — contain only SHA256 hash hex strings |
| Search index files | No | Already opaque — contain only SHA256 hashes and hex keys |

### Why Search Still Works

1. `ObjectDataSearchIndexer` computes `SHA256(propertyValue)` from the **in-memory** value before any storage write occurs. Index files map these hashes to object keys.
2. `ObjectDataSearcher` encodes search criteria the same way, looks up index files, then calls `ReadObjectDataAsync(key)` which goes through the storage manager — where `LoadSlot` decrypts transparently.
3. Search index files never contain plaintext property values, only SHA256 hashes and hex keys.

### Implementation

- `EncryptingFsSlottedStorage` extends `FsSlottedStorage`, overriding `Save(IStorageSlot, IRawData)` to encrypt before writing and `LoadSlot(IStorageSlot)` to decrypt after reading.
- `EncryptedFsObjectDataStorageManager` extends `FsObjectDataStorageManager`, overriding `GetRawStorage()` to return an `EncryptingFsSlottedStorage` instance.

### DI Wiring

**Plaintext (default):**
```csharp
serviceRegistry.For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
```

**Encrypted:**
```csharp
serviceRegistry
    .For<IAesKeySource>().Use(new AesKey())
    .For<IEncryptor>().Use<AesEncryptor>()
    .For<IDecryptor>().Use<AesDecryptor>()
    .For<IObjectDataStorageManager>().Use<EncryptedFsObjectDataStorageManager>();
```

The only change required is swapping the `IObjectDataStorageManager` registration and providing encryption dependencies. All other registrations remain identical.
