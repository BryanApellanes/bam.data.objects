using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Data.Objects.Tests.Integration;

[UnitTestMenu("Integration: ObjectDataWriter should")]
public class ObjectDataWriterShould: UnitTestMenuContainer
{
    public ObjectDataWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task WriteData()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(WriteData));
        PlainTestClass plainTestClass = new PlainTestClass(true);

        When.A<ObjectDataWriter>("writes data",
            () =>
            {
                ServiceRegistry testContainer = ConfigureDependencies(root);
                testContainer
                    .For<IPropertyWriter>().Use<PropertyWriter>()
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>()
                    .For<IHashCalculator>().Use<JsonHashCalculator>()
                    .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
                    .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
                    .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
                    .For<IObjectDataFactory>().Use<ObjectDataFactory>();
                return testContainer.Get<ObjectDataWriter>();
            },
            (objectDataWriter) =>
            {
                return objectDataWriter.WriteAsync(plainTestClass).GetAwaiter().GetResult();
            })
        .TheTest
        .ShouldPass(because =>
        {
            IObjectDataWriteResult result = (IObjectDataWriteResult)because.Result;
            because.ItsTrue("result is not null", result != null);
            because.ItsTrue("result succeeded", result?.Success == true);
            because.ItsTrue("result.ObjectData is not null", result?.ObjectData != null);
            because.ItsTrue("result.ObjectData.Data is the original object", ReferenceEquals(result?.ObjectData?.Data, plainTestClass));
            because.ItsTrue("result.ObjectDataKey is not null", result?.ObjectDataKey != null);
            because.ItsTrue("result.KeySlot is not null", result?.KeySlot != null);
            because.ItsTrue("result has 4 PropertyWriteResults", result?.PropertyWriteResults?.Count == 4);

            if (result?.PropertyWriteResults != null)
            {
                foreach (string key in result.PropertyWriteResults.Keys)
                {
                    IPropertyWriteResult propertyWriteResult = result.PropertyWriteResults[key];
                    because.ItsTrue($"propertyWriteResult[{key}] is not null", propertyWriteResult != null);
                    because.ItsTrue($"propertyWriteResult[{key}].Property is not null", propertyWriteResult?.Property != null);
                    because.ItsTrue($"propertyWriteResult[{key}].RawData is not null", propertyWriteResult?.RawData != null);
                }
            }
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    public async Task WritePropertyFiles()
    {
        throw new NotImplementedException();
    }

    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath);
        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}
