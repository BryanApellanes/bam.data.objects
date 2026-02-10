using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("FsObjectStorageManager Should")]
public class FsObjectStorageManagerShould(ServiceRegistry serviceRegistry) : UnitTestMenuContainer(serviceRegistry)
{
    [UnitTest]
    public Task WriteAndReadProperty()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteAndReadProperty));
        string stringPropertyValue = null;

        When.A<FsObjectDataStorageManager>("writes and reads a property",
            () =>
            {
                ServiceRegistry svcRegistry = IntegrationTests.ConfigureDependencies(root);
                svcRegistry
                    .For<IObjectDecoder>().Use<JsonObjectDataEncoder>()
                    .For<IPropertyWriter>().Use<PropertyWriter>()
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return svcRegistry.Get<FsObjectDataStorageManager>();
            },
            (fsObjectDataStorageManager) =>
            {
                IObjectDataFactory objectDataFactory = IntegrationTests.ConfigureDependencies(root)
                    .For<IObjectDecoder>().Use<JsonObjectDataEncoder>()
                    .For<IPropertyWriter>().Use<PropertyWriter>()
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>()
                    .Get<IObjectDataFactory>();
                IObjectData testObjectData = objectDataFactory.GetObjectData(new PlainTestClass(true));
                IProperty stringProperty = testObjectData.Property(nameof(PlainTestClass.StringProperty));
                stringPropertyValue = stringProperty?.Value;
                IPropertyWriteResult propertyWriteResult = fsObjectDataStorageManager.WriteProperty(stringProperty!);
                IProperty readProperty = fsObjectDataStorageManager.ReadProperty(
                    new ObjectData(propertyWriteResult.ObjectDataKey.TypeDescriptor),
                    propertyWriteResult.GetDescriptor());
                return readProperty.Value;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .IsEqualTo(stringPropertyValue);
        })
        .SoBeHappy()
        .UnlessItFailed();

        return Task.CompletedTask;
    }
}
