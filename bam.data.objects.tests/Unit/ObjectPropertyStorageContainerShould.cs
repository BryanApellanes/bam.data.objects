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
        ObjectPropertyStorageHolder objectPropertyStorageHolder =
            new ObjectPropertyStorageHolder(testDataPath);
        objectPropertyStorageHolder.FullName.ShouldEqual(expectedPath);
    }
    
    [UnitTest]
    public void HaveVersion()
    {
        ObjectPropertyStorageHolder objectPropertyStorageHolder =
            new ObjectPropertyStorageHolder(Path.Combine(Environment.CurrentDirectory, nameof(HaveVersion), "testData"));
        objectPropertyStorageHolder.Version.ShouldNotBeNull();
        objectPropertyStorageHolder.Version.Number.ShouldEqual(1);
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
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath));

        ServiceRegistry serviceRegistry = Configure(testRegistry);
        return serviceRegistry;
    }
}