# bam.data.objects.tests

Unit and integration tests for the bam.data.objects library.

## Overview

This project contains tests for the filesystem-backed object persistence layer provided by `bam.data.objects`. Tests are organized into Unit and Integration folders and are written using the Bam test framework with `[UnitTestMenu]` attributes and the `When.A<T>()` fluent API. The test runner is menu-driven via `BamConsoleContext.StaticMain`.

Unit tests verify core components in isolation (type identifiers, object keys, properties, object data factories, writers, property storage holders, and storage managers). Integration tests exercise the full storage pipeline end-to-end, including filesystem object storage managers, property writers, property holders, object data writers, and object repository operations.

## Key Classes

| Class | Description |
|---|---|
| `UnitTests` | Static helper class that configures the DI `ServiceRegistry` with all required implementations for unit testing (hash calculators, factories, encoders, storage holders). |
| `IntegrationTests` | Static helper class that configures the DI `ServiceRegistry` for integration tests, adding `ObjectDataReader`, `FsStorageHolder`, and other filesystem-dependent implementations. |
| `TypeIdentifierShould` | Unit tests for type identifier behavior. |
| `ObjectKeyShould` | Unit tests for object key computation. |
| `PropertyShould` | Unit tests for property value handling. |
| `ObjectDataShould` | Unit tests for `ObjectData` wrapping and property discovery. |
| `ObjectDataFactoryShould` | Unit tests for `ObjectDataFactory` behavior. |
| `ObjectIdentifierFactoryShould` | Unit tests for object identifier factory. |
| `ObjectDataWriterShould` | Unit tests for `ObjectDataWriter` (unit-level). |
| `PropertyStorageHolderShould` | Unit tests for property storage holder path resolution. |
| `ObjectStorageManagerShould` | Unit tests for storage manager operations. |
| `FsObjectStorageManagerShould` | Integration tests for filesystem-backed object storage. |
| `PropertyWriterShould` | Integration tests for writing properties to filesystem storage. |
| `PropertyHolderShould` | Integration tests for property holder operations. |
| `ObjectRepositoryShould` | Integration tests for object repository CRUD operations. |
| `PlainTestClass` | Simple POCO used as a test fixture. |
| `TestRepoData` | Test data class extending `RepoData` for repository testing. |

## Dependencies

### Project References
- bam.base
- bam.shell
- bam.test
- bam.data.objects
- bam.encryption

### Package References
- NSubstitute 5.3.0

### Target Framework
- net10.0 (Exe)

## Running Tests

```bash
dotnet run --project bam.data.objects.tests.csproj -- --ut
```

**Important**: Use `--ut` (not `/ut`) when running from Git Bash, as Git Bash rewrites `/ut` to a filesystem path.

## Known Gaps / Not Yet Implemented

No significant gaps identified in the test project itself. Coverage gaps correspond to the stubs in the main library (`PropertyReader`, `ObjectDataArchiver`).
