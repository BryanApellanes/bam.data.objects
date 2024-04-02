using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Net.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("Fs Object Property Writer should")]
public class FsObjectPropertyWriterShould : UnitTestMenuContainer
{
    public FsObjectPropertyWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {

    }

    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<ObjectFsRootDirectory>().Use(new ObjectFsRootDirectory($"{Path.Combine(Environment.CurrentDirectory, $"{nameof(FsObjectPropertyWriterShould)}_Tests")}"))
            .For<IObjectFs>().Use<ObjectFs>()
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>()
            .For<IObjectConverter>().Use<JsonObjectEncoder>();
    }
    
    [UnitTest]
    public async Task CreateExpectedDirectories()
    {
        FsObjectPropertyWriter propertyWriter = Get<FsObjectPropertyWriter>();
        string[] expected = new string[] {propertyWriter.ObjectFs.GetRootDirectory().FullName,"objects", "Bam","Data","Dynamic","TestClasses","TestData","local","id"};
        string path = Path.Combine(expected);
        if (Directory.Exists(path))
        {
            Directory.Delete(path);
        }
        // Object properties
        // {root}/objects/name/space/type/local/id/{Id}/{propertyName}/{version}/hash.dat
        TestData testData = new TestData
        {
            StringProperty = 32.RandomLetters()
        };
        await propertyWriter.WritePropertyAsync(typeof(TestData).GetProperty("StringProperty"), testData);
        Directory.Exists(Path.Combine(expected)).ShouldBeTrue();
    }
}