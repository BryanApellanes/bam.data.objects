using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Storage;
using Bam.Testing;
using NSubstitute;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectFs Should", "ofs")]
public class ObjectStorageManagerShould : UnitTestMenuContainer
{
    public ObjectStorageManagerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
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
        ObjectStorageManager ofs = this.Get<ObjectStorageManager>();
        dynamic ob = new
        {
        };
        
        DirectoryInfo directoryInfo = ofs.GetTypeStorage(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.FullName);
    }

    [UnitTest]
    public void GetTypeDirectory()
    {
        ObjectStorageManager ofs = this.Get<ObjectStorageManager>();
        TestData ob = new TestData();
        
        IStorageIdentifier directoryInfo = ofs.GetTypeStorage(ob.GetType());
        directoryInfo.ShouldNotBeNull();
        Message.PrintLine("typeDir = '{0}'", directoryInfo.Value);
    }
    // Objects
    // {root}/objects/name/space/type/key/{Key}.dat -> {ObjectIdentifierHash}
    
    // Object properties
    // {root}/objects/name/space/type/hash/{ObjectIdentifierHash}/{propertyName}/{version}/val.dat content -> {RawDataHash}
    
    // Raw data
    // {root}/raw/{hash}.dat   
    
    [UnitTest]
    public void GetDirectories()
    {
        
    }
}