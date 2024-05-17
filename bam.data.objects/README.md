## Storage
When an object is written to file system storage the following files are written:

For each property of the object:
- A dat file which contains the property value:
  - {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat

## Indexing

When an object is indexed the following file is written:

For each property of the object:
- A dat file which contains the ObjectKey:
  - {root}/index/name/space/type/{propertyName}/map_{num}.dat
  
Map file content is in the following form:

```
{value_1}: {ObjectKey}
{value_2}: {ObjectKey}
...
```

## Deleting

When an object is deleted the following file is written:
- A file containing the time of deletion and the user who deleted the entry:
  - {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/deleted

## Archiving

When an object is archived all deleted objects are moved to the archive folder maintaining their original relative path:
- {root}/archive/name/space/type/{Ob/je/ct/Ke/y_}/*

## Loading

When an object is loaded the ObjectKey is used to identify the object to load.

## Searching

When objects are searched an ObjectSearch is used to provide search criteria for objects to load.