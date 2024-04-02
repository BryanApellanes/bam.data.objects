using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Net.CoreServices;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectFs Should", "ofs")]
public class ObjectFsShould : UnitTestMenuContainer
{
    public ObjectFsShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>();
    }

    [UnitTest]
    public void GetTypeDirectoryForDynamicType()
    {
        ObjectFs ofs = this.Get<ObjectFs>();
        dynamic ob = new
        {
        };
        
        DirectoryInfo directoryInfo = ofs.GetTypeDirectory(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetTypeDirectory()
    {
        ObjectFs ofs = this.Get<ObjectFs>();
        TestData ob = new TestData();
        
        DirectoryInfo directoryInfo = ofs.GetTypeDirectory(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }
    // Object properties
    // {root}/objects/name/space/type/local/id/{Id}/{propertyName}/{version}/val.dat content -> {hash}
    
    // {root}/objects/name/space/type/keys/{Key}/{propertyName}/{version}/val.dat content -> {hash}
    // Raw data
    // {root}/raw/{hash}.dat   
    
    [UnitTest]
    public void GetDirectories()
    {
        
    }
}