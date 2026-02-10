using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Data.Objects.Tests.Integration;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectDataFactoryShould")]
public class ObjectDataFactoryShould : UnitTestMenuContainer
{
    public ObjectDataFactoryShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task SetObjectIdentifierFactoryOnObjectData()
    {
        string rootPath = Path.Combine(Environment.CurrentDirectory, nameof(SetObjectIdentifierFactoryOnObjectData));

        When.A<ObjectDataFactory>("creates ObjectData from a PlainTestClass",
            () =>
            {
                ServiceRegistry testRegistry = IntegrationTests.ConfigureDependencies(rootPath)
                    .For<IObjectDataStorageManager>().Use<FsObjectDataStorageManager>();
                return testRegistry.Get<ObjectDataFactory>();
            },
            (dataFactory) =>
            {
                IObjectData objectData = dataFactory.GetObjectData(new PlainTestClass(true));
                return new object[] { objectData, objectData.GetObjectKey() };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            IObjectData objectData = (IObjectData)results[0];
            IObjectDataKey objectDataKey = (IObjectDataKey)results[1];
            because.ItsTrue("ObjectDataLocatorFactory is not null", objectData.ObjectDataLocatorFactory != null);
            because.ItsTrue("objectKey is not null", objectDataKey != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
