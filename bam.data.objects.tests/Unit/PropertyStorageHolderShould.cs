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
public class PropertyStorageHolderShould: UnitTestMenuContainer
{
    public PropertyStorageHolderShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void HaveVersionPath()
    {
        string testDataPath = Path.Combine(Environment.CurrentDirectory, nameof(HaveVersionPath), "testData");
        string expectedPath = Path.Combine(testDataPath, "1");
        PropertyStorageHolder propertyStorageHolder =
            new PropertyStorageHolder(testDataPath);
        propertyStorageHolder.FullName.ShouldEqual(expectedPath);
    }
    
    [UnitTest]
    public void HaveVersion()
    {
        PropertyStorageHolder propertyStorageHolder =
            new PropertyStorageHolder(Path.Combine(Environment.CurrentDirectory, nameof(HaveVersion), "testData"));
        propertyStorageHolder.Version.ShouldNotBeNull();
        propertyStorageHolder.Version.Number.ShouldEqual(1);
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