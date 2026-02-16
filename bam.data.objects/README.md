# bam.data.objects

Filesystem-backed object persistence with content-addressed property values, revision tracking, search indexing, and transparent encryption support.

## Overview

`bam.data.objects` provides a storage-agnostic object data system built around the concepts of object identity, property-level storage with revision history, and content-addressable keys. Rather than mapping objects to relational database tables, this library serializes object properties into a hierarchical filesystem structure, where each property value is stored as an individually addressable file identified by a hash-based key.

The core abstraction is `IObjectData`, which wraps any plain C# object and provides access to its properties as `IProperty` instances. Each property can be individually written, read, and versioned. Objects are identified by composite keys computed from their property values (via `ICompositeKeyCalculator`), and indexed by both sequential ID and UUID for fast lookup. The `ObjectDataSearcher` supports multi-criteria search with operators including Equals, StartsWith, EndsWith, Contains, and their negations.

The library also includes an encryption layer (via the `bam.encryption` dependency) allowing transparent encryption of stored property data, and a JSON-based encoder/decoder (`JsonObjectDataEncoder`) for serializing property values to raw storage.

## Storage Architecture

### Pointer Slots (Revision System)

Each object property is stored as a series of **revision slots**. A revision slot is a small file containing a SHA256 hash hex string that points to the actual property value in the raw content-addressed store.

```
<root>/objects/<TypePath>/<KeySegment1>/<KeySegment2>/<PropertyName>/1  -> pointer (hash hex string)
<root>/objects/<TypePath>/<KeySegment1>/<KeySegment2>/<PropertyName>/2  -> pointer (revision 2)
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
- **Search index**: `ObjectDataSearchIndexer` creates index files mapping `SHA256(propertyValue)` to object keys. This enables search-by-property-value without scanning all objects.

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

When objects are searched an `ObjectDataSearch` is used to provide search criteria. The `ObjectDataSearcher` supports the following operators:

| Operator | Description | Strategy |
|---|---|---|
| `Equals` | Exact match (default) | Hash-based index lookup -- O(1) |
| `StartsWith` | Value starts with search term | In-memory scan -- O(n) |
| `EndsWith` | Value ends with search term | In-memory scan -- O(n) |
| `Contains` | Value contains search term | In-memory scan -- O(n) |
| `DoesntStartWith` | Value does not start with search term | In-memory scan -- O(n) |
| `DoesntEndWith` | Value does not end with search term | In-memory scan -- O(n) |
| `DoesntContain` | Value does not contain search term | In-memory scan -- O(n) |

All string comparisons for non-Equals operators use `OrdinalIgnoreCase`.

`Equals` uses SHA256 hash-based index lookup for fast O(1) matching. All other operators load objects via `IObjectDataReader` (which handles decryption transparently) and compare property values in memory. No cleartext property values are stored on disk -- search indexes remain opaque.

```csharp
ObjectDataSearch search = new ObjectDataSearch(typeof(MyType))
    .Where("Name", "John", SearchOperator.StartsWith)
    .Where("Status", "Active", SearchOperator.Equals);
```

Multiple criteria use AND semantics -- each criterion produces a key set and results are intersected.

## Key Classes

| Class | Description |
|---|---|
| `ObjectData` / `ObjectData<T>` | Wraps a plain object, discovers its properties via reflection, provides property get/set, encoding, and key generation. |
| `IObjectData` | Core interface for wrapped object data: property access, encoding, key and identifier generation. |
| `ObjectDataWriter` | Async writer that persists `IObjectData` instances via `IObjectDataStorageManager`. |
| `ObjectDataReader` | Async reader that loads `IObjectData` from storage by `IObjectDataKey`. |
| `ObjectDataDeleter` | Async deleter that removes object storage directories and index files. |
| `ObjectDataSearcher` | Multi-criteria async search: hash-based lookup for equality, in-memory scan for pattern operators, with AND semantics. |
| `ObjectDataIndexer` | Creates and queries filesystem-based index files mapping sequential IDs and UUIDs to storage keys. |
| `ObjectDataFactory` | Factory that wraps raw objects as `IObjectData`, delegating to `IObjectDataLocatorFactory` and `IObjectEncoderDecoder`. |
| `PropertyWriter` | Writes individual property values to versioned storage slots. |
| `PropertyReader` | (Stub) Intended to read property values from storage; currently throws `NotImplementedException`. |
| `Property` | Represents a named property with a value, parent object reference, and descriptor. Supports reconstruction from raw data. |
| `PropertyStorageHolder` / `PropertyStorageRevisionHolder` | Filesystem holders for property storage directories and revision subdirectories. |
| `PropertyStorageRevisionSlot` | A specific versioned slot within a property's revision storage. |
| `ObjectDataKey` / `ObjectDataKey<T>` | Identifies an object by its `TypeDescriptor` and hex-encoded composite key. |
| `TypeDescriptor` | Describes a type by its `Type` reference, used for storage path resolution. |
| `CompositeKeyCalculator` | Computes composite keys from properties decorated with `[CompositeKey]` or all properties if none are decorated. |
| `JsonObjectDataEncoder` | Default encoder/decoder that serializes objects as JSON and provides hash-based identity calculation. |
| `ObjectDataArchiver` | (Stub) Intended for archiving object data; currently throws `NotImplementedException`. |

## Dependencies

### Project References
- bam.base
- bam.configuration
- bam.data.repositories
- bam.data.schema
- bam.data.config
- bam.data.firebird
- bam.data.mssql
- bam.data.mysql
- bam.data.oracle
- bam.data.postgres
- bam.data
- bam.logging
- bam.storage
- bam.encryption

### Package References
- Newtonsoft.Json 13.0.4

### Target Framework
- net10.0

## Encrypted (Opaque) Storage

`EncryptedFsObjectDataStorageManager` provides transparent encryption of property values at rest. The design encrypts only the raw content-addressed store, leaving pointer data and search indexes untouched.

### What Gets Encrypted

| Component | Encrypted? | Reason |
|---|---|---|
| Raw property values (`<root>/raw/`) | Yes | Contains actual data (JSON-serialized values) |
| Pointer slots (revision files) | No | Already opaque -- contain only SHA256 hash hex strings |
| Search index files | No | Already opaque -- contain only SHA256 hashes and hex keys |

### Why Search Still Works

1. `ObjectDataSearchIndexer` computes `SHA256(propertyValue)` from the **in-memory** value before any storage write occurs. Index files map these hashes to object keys.
2. `ObjectDataSearcher` encodes search criteria the same way, looks up index files, then calls `ReadObjectDataAsync(key)` which goes through the storage manager -- where `LoadSlot` decrypts transparently.
3. Search index files never contain plaintext property values, only SHA256 hashes and hex keys.

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

## Usage Examples

### Writing an object
```csharp
IObjectDataFactory factory = serviceRegistry.Get<IObjectDataFactory>();
IObjectDataStorageManager storageManager = serviceRegistry.Get<IObjectDataStorageManager>();

var writer = new ObjectDataWriter(factory, storageManager);
var myObject = new MyClass { Name = "Example", Value = 42 };

IObjectDataWriteResult result = await writer.WriteAsync(myObject);
```

### Reading an object by key
```csharp
var reader = new ObjectDataReader(storageManager);
IObjectDataKey key = objectData.GetObjectKey();

IObjectDataReadResult result = await reader.ReadObjectDataAsync(key);
IObjectData loaded = result.ObjectData;
```

### Searching for objects
```csharp
var searcher = new ObjectDataSearcher(searchIndexer, reader, indexer);
var search = new ObjectDataSearch(typeof(MyClass))
    .Where("Name", "Example", SearchOperator.Equals);

IObjectDataSearchResult result = await searcher.SearchAsync(search);
foreach (IObjectData match in result.Results)
{
    Console.WriteLine(match.ToJson());
}
```

### Deleting an object
```csharp
var deleter = new ObjectDataDeleter(factory, storageManager, compositeKeyCalculator);
IObjectDataDeleteResult deleteResult = await deleter.DeleteAsync(myObject);
```

## Known Gaps / Not Yet Implemented

- **`PropertyReader`**: Both `ReadProperty` overloads throw `NotImplementedException`. Property reading is handled through `IObjectDataStorageManager.ReadProperty` instead.
- **`ObjectDataArchiver`**: Both `ArchiveAsync` overloads throw `NotImplementedException`. No archival functionality is currently available.
- **`ObjectPersister.cs` and `ObjectReader.cs`**: Excluded from compilation in the .csproj (listed under `<Compile Remove>`), indicating these are legacy or incomplete implementations.
