using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Net.CoreServices;
using Bam.Net.Data.Repositories;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("Object writer should", "ows")]
public class ObjectWriterShould : UnitTestMenuContainer
{
    public ObjectWriterShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }
    
    public override ServiceRegistry Configure(ServiceRegistry serviceRegistry)
    {
        return base.Configure(serviceRegistry)
            .For<ObjectFsRootDirectory>().Use(new ObjectFsRootDirectory(Path.Combine(Environment.CurrentDirectory, "testRoot")))
            .For<IObjectPropertyWriter>().Use<FsObjectPropertyWriter>()
            .For<IObjectConverter>().Use<JsonObjectEncoder>()
            .For<IObjectFs>().Use<ObjectFs>()
            .For<IObjectHashCalculator>().Use<ObjectHashCalculator>()
            .For<IHashCalculator>().Use<JsonHashCalculator>()
            .For<IKeyHashCalculator>().Use<CompositeKeyHashCalculator>();
    }
    
    [UnitTest]
    public async Task FireWriteObjectStartedEvent()
    {
        AutoResetEvent waitSignal = new AutoResetEvent(false);
        ObjectWriter objectWriter = Get<ObjectWriter>();
        TestData data = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 10.RandomLetters(),
            LongProperty = RandomNumber.Between(1, 100),
            DateTimeProperty = DateTime.Now
        };
        
        bool? startedEventFired = false;
        objectWriter.WriteObjectStarted += (sender, args) =>
        {
            startedEventFired = true;
            waitSignal.Set();
        };
        
        objectWriter.Enqueue(data);
        waitSignal.WaitOne(3000);
        startedEventFired.ShouldBeTrue("Started event didn't fire");
    }
    
    [UnitTest]
    public async Task FireWriteObjectCompleteEvent()
    {
        AutoResetEvent waitSignal = new AutoResetEvent(false);
        ObjectWriter objectWriter = Get<ObjectWriter>();
        TestData data = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 10.RandomLetters(),
            LongProperty = RandomNumber.Between(1, 100),
            DateTimeProperty = DateTime.Now
        };
        
        bool? completeEventFired = false;
        objectWriter.WriteObjectComplete += (sender, args) =>
        {
            completeEventFired = true;
            waitSignal.Set();
        };
        
        objectWriter.Enqueue(data);
        waitSignal.WaitOne(3000);
        completeEventFired.ShouldBeTrue("Complete event didn't fire");
    }

    [UnitTest]
    public void CreateExpectedDirectories()
    {
        // Object properties
        // {root}/objects/name/space/type/local/id/{Id}/{propertyName}/{version}/val.dat
        // {root}/objects/name/space/type/keys/{Key}/id.dat
        // Raw data
        // {root}/raw/{rawHash}.dat     
    }
}