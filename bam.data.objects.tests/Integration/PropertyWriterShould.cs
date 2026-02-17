using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("PropertyWriterShould")]
public class PropertyWriterShould: UnitTestMenuContainer
{
    public PropertyWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WritePropertyDatFile()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WritePropertyDatFile));
        string propertyName = "StringProperty";
        string? expectedPath = null;

        When.A<PropertyWriter>("writes a property dat file",
            () =>
            {
                ServiceRegistry testContainer = IntegrationTests.ConfigureDependencies(root);
                testContainer
                    .For<IPropertyWriter>().Use<PropertyWriter>()
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return testContainer.Get<PropertyWriter>();
            },
            (propertyWriter) =>
            {
                ServiceRegistry testContainer = IntegrationTests.ConfigureDependencies(root);
                testContainer
                    .For<IPropertyWriter>().Use<PropertyWriter>()
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();

                IObjectDataFactory dataFactory = testContainer.Get<ObjectDataFactory>();
                IObjectData testData = dataFactory.GetObjectData(new PlainTestClass(true));
                IObjectDataKey objectDataKey = testData.GetObjectKey();
                IObjectDataStorageManager objectDataStorageManager = testContainer.Get<FsObjectDataStorageManager>();

                IProperty property = testData.Property(propertyName)!;
                int nextVersion = objectDataStorageManager.GetNextRevisionNumber(property);

                List<string> expectedParts = new List<string>();
                expectedParts.Add(objectDataKey.GetPath(objectDataStorageManager));
                expectedParts.Add(propertyName);
                expectedParts.Add(nextVersion.ToString());
                expectedParts.Add("dat");
                expectedPath = Path.Combine(expectedParts.ToArray());

                IPropertyWriteResult result = propertyWriter.WritePropertyAsync(property).GetAwaiter().GetResult();
                return new object[]
                {
                    objectDataKey.GetPath(objectDataStorageManager).StartsWith(root),
                    property != null,
                    result.PointerStorageSlot.FullName!
                };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            because.ItsTrue("objectKey path is in correct root", (bool)results[0]);
            because.ItsTrue("property is not null", (bool)results[1]);
            because.ItsTrue("result path equals expected", expectedPath!.Equals((string)results[2]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
