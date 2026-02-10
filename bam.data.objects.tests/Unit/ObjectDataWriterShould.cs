using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Data.Repositories;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;
using Bam.Test;
using Bamn.Data.Objects;
using NSubstitute;

namespace Bam.Application.Unit;

[UnitTestMenu("Unit: ObjectDataWriter should")]
public class ObjectDataWriterShould: UnitTestMenuContainer
{
    public ObjectDataWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task CallObjectIdentifierGetObjectKey()
    {
        IObjectDataLocatorFactory mockObjectDataLocatorFactory = Substitute.For<IObjectDataLocatorFactory>();
        IObjectDataStorageManager mockDataStorageManager = Substitute.For<IObjectDataStorageManager>();

        When.A<ObjectDataWriter>("writes ObjectData",
            () =>
            {
                ServiceRegistry testContainer = new ServiceRegistry()
                    .For<IObjectDataStorageManager>().Use(mockDataStorageManager)
                    .For<IObjectDataLocatorFactory>().Use(mockObjectDataLocatorFactory)
                    .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
                    .For<IObjectDataFactory>().Use<ObjectDataFactory>();
                return testContainer.Get<ObjectDataWriter>();
            },
            (objectDataWriter) =>
            {
                PlainTestClass plainTestClass = new PlainTestClass();
                ObjectData objectData = new ObjectData(plainTestClass);
                objectDataWriter.WriteAsync(objectData).GetAwaiter().GetResult();
                return new object[] { objectData, objectData.ObjectDataLocatorFactory };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            ObjectData objectData = (ObjectData)results[0];
            object locatorFactory = results[1];
            mockDataStorageManager.Received().WriteObject(objectData);
            because.ItsTrue("ObjectDataLocatorFactory is the mock", ReferenceEquals(locatorFactory, mockObjectDataLocatorFactory));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    // Disabled: intentionally throws, pending redesign to use IObjectDataIndexer
    public async Task WriteKeyFile()
    {
        throw new InvalidOperationException(
            "Review this test for validity.  Why should a key file be written to the expected path" +
            "This should probably be replaced by the concept of an IObjectDataIndexer");
    }

    [UnitTest]
    public async Task CallObjectStorageManagerGetRootStorage()
    {
        IObjectDataStorageManager mockDataStorageManager = Substitute.For<IObjectDataStorageManager>();

        When.A<ObjectDataWriter>("writes ObjectData and calls storage manager",
            () =>
            {
                ServiceRegistry testRegistry = new ServiceRegistry()
                    .For<IHashCalculator>().Use<JsonHashCalculator>()
                    .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
                    .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
                    .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
                    .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
                    .For<IObjectDataFactory>().Use<ObjectDataFactory>()
                    .For<IObjectDataStorageManager>().Use(mockDataStorageManager);
                return testRegistry.Get<ObjectDataWriter>();
            },
            (objectDataWriter) =>
            {
                ObjectData objectData = new ObjectData(new PlainTestClass());
                objectDataWriter.WriteAsync(objectData).GetAwaiter().GetResult();
                return objectData;
            })
        .TheTest
        .ShouldPass(because =>
        {
            ObjectData objectData = (ObjectData)because.Result;
            mockDataStorageManager.Received().WriteObject(objectData);
            because.ItsTrue("storage manager WriteObject was called", true);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IRootStorageHolder>().Use( new RootStorageHolder(rootPath))
            .For<IStorageIdentifier>().Use(new FsStorageHolder(rootPath));

        ServiceRegistry dependencyProvider = Configure(testRegistry);
        return dependencyProvider;
    }
}
