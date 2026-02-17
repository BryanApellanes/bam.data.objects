using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("PropertyHolderShould")]
public class PropertyHolderShould: UnitTestMenuContainer
{
    public PropertyHolderShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WriteDatFileToExpectedPath()
    {
        string rootPath = Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty));
        string? expected = null;

        When.A<PropertyStorageHolder>("writes a dat file to expected path",
            () =>
            {
                ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return testRegistry.Get<PropertyStorageHolder>(new string[] { rootPath });
            },
            (storageHolder) =>
            {
                ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                ObjectDataFactory dataFactory = testRegistry.Get<ObjectDataFactory>();
                IObjectDataStorageManager dataStorageManager = testRegistry.Get<IObjectDataStorageManager>();

                IObjectData objectData = dataFactory.GetObjectData(new PlainTestClass
                {
                    StringProperty = "StringProperty-SaveObjectPropertyTest"
                });

                IObjectDataKey dataKey = objectData.GetObjectKey();

                List<string> pathSegments = new List<string> { rootPath, "objects" };
                pathSegments.AddRange(typeof(PlainTestClass).FullName!.Split('.'));
                pathSegments.AddRange(dataKey.Key!.Split(2));
                pathSegments.Add("StringProperty");
                pathSegments.Add("1");
                pathSegments.Add("dat");

                expected = Path.Combine(pathSegments.ToArray());
                if (File.Exists(expected))
                {
                    File.Delete(expected);
                }
                IPropertyWriteResult writeResult = storageHolder.Save(dataStorageManager, objectData.Property("StringProperty")!);
                return new object[] { writeResult.PointerStorageSlot.FullName!, File.Exists(writeResult.PointerStorageSlot.FullName) };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            because.ItsTrue("write result path equals expected", expected!.Equals((string)results[0]));
            because.ItsTrue("file exists", (bool)results[1]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SaveObjectProperty()
    {
        string rootPath = Path.Combine(Environment.CurrentDirectory, nameof(SaveObjectProperty));

        When.A<FsObjectDataStorageManager>("saves an object property",
            () =>
            {
                ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return testRegistry.Get<FsObjectDataStorageManager>();
            },
            (dataStorageManager) =>
            {
                ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                ObjectDataFactory dataFactory = testRegistry.Get<ObjectDataFactory>();
                IObjectData objectData = dataFactory.GetObjectData(new PlainTestClass
                {
                    StringProperty = "StringProperty-SaveObjectPropertyTest"
                });
                IProperty property = objectData.Property("StringProperty")!;
                IPropertyStorageHolder propertyStorageHolder =
                    dataStorageManager.GetPropertyStorageHolder(property.ToDescriptor());
                IPropertyWriteResult writeResult = propertyStorageHolder.Save(dataStorageManager, property);
                return new object[] { writeResult.PointerStorageSlot.FullName!, File.Exists(writeResult.PointerStorageSlot.FullName) };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            because.ItsTrue("file exists", (bool)results[1]);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
