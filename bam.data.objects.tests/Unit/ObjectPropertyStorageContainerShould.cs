using System.Text;
using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectPropertyStorageContainerShould")]
public class ObjectPropertyStorageContainerShould: UnitTestMenuContainer
{
    public ObjectPropertyStorageContainerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void HaveVersionPath()
    {
        string testDataPath = Path.Combine(Environment.CurrentDirectory, nameof(HaveVersionPath), "testData");
        string expectedPath = Path.Combine(testDataPath, "1");
        ObjectPropertyStorageContainer objectPropertyStorageContainer =
            new ObjectPropertyStorageContainer(testDataPath);
        objectPropertyStorageContainer.FullName.ShouldEqual(expectedPath);
    }
    
    [UnitTest]
    public void HaveVersion()
    {
        ObjectPropertyStorageContainer objectPropertyStorageContainer =
            new ObjectPropertyStorageContainer(Path.Combine(Environment.CurrentDirectory, nameof(HaveVersion), "testData"));
        objectPropertyStorageContainer.Version.ShouldNotBeNull();
        objectPropertyStorageContainer.Version.Number.ShouldEqual(1);
    }
    
    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<IObjectCalculator>().Use<ObjectCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyCalculator>().Use<CompositeKeyCalculator>();
    }
    
    private ServiceRegistry ConfigureDependencies(string rootPath)
    {
        ServiceRegistry testRegistry = new ServiceRegistry()
            .For<IRootStorageContainer>().Use(new RootStorageContainer(rootPath));

        ServiceRegistry serviceRegistry = Configure(testRegistry);
        return serviceRegistry;
    }
}