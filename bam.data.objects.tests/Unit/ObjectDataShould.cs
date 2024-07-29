using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.CoreServices;
using Bam.Shell;
using Bam.Storage;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectData should")]
public class ObjectDataShould : UnitTestMenuContainer
{
    public ObjectDataShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task RecieveArgument(IMenuManager menuManager)
    {
        Message.PrintLine("There are {0} menus",menuManager.Menus.Count.ToString());
    }
    
    [UnitTest]
    public void HaveType()
    {
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 16.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };

        ObjectData data = new ObjectData(testData);
        
        data.TypeDescriptor.ShouldNotBeNull("Type was null");
        data.TypeDescriptor.Type.ShouldNotBeNull("TypeIdentifier.Type was null");
        data.TypeDescriptor.Type.ShouldBe(typeof(TestData));
        data.TypeDescriptor.AssemblyQualifiedTypeName.ShouldNotBeNull("TypeIdentifier.AssemblyQualifiedTypeName was null");
    }

    [UnitTest]
    public void BeIObjectData()
    {
        (new ObjectData("test"){} is IObjectData).ShouldBeTrue();
    }
    
    [UnitTest]
    public void HaveProperties()
    {
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 16.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };

        ObjectData data = new ObjectData(testData);
        
        data.Properties.ShouldNotBeNull("data.Properties was null");
        int propCount = data.Properties.Count();
        propCount.ShouldEqual(4, $"data.Properties.Count() was {propCount} instead of 4");
    }

    [UnitTest]
    public void OutputSameJson()
    {
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 16.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };

        ObjectData testObjectData = new ObjectData(testData);

        string expected = testData.ToJson();
        string actual = testObjectData.ToJson();
        
        expected.ShouldEqual(actual);
        Message.PrintLine(expected);
    }

    [UnitTest]
    public void GetIntPropertyValue()
    {
        int expected = RandomNumber.Between(100, 1000);
        TestData testData = new TestData
        {
            IntProperty = expected,
            StringProperty = 8.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };

        ObjectData testObjectData = new ObjectData(testData);
        IProperty property = testObjectData.Property("IntProperty");
        object decoded = property.Decode();
        // convert to long because the decoded value of a number 
        // gets converted to long by the underlying deserialization
        (decoded).ShouldEqual((long)expected);
    }
    
    [UnitTest]
    public void GetStringPropertyValue()
    {
        string expected = 32.RandomLetters();
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = expected,
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };

        ObjectData testObjectData = new ObjectData(testData);
        IProperty property = testObjectData.Property("StringProperty");
        property.Decode().ShouldEqual(expected);
    }
    
    [UnitTest]
    public void SetPropertyValue()
    {
        string expected = 32.RandomLetters();
        TestData testData = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 6.RandomLetters(),
            LongProperty = RandomNumber.Between(100, 1000),
            DateTimeProperty = DateTime.Now
        };

        ObjectData testObjectData = new ObjectData(testData);
        IProperty getProperty = testObjectData.Property("StringProperty");
        getProperty.Decode().ShouldNotEqual(expected);
        IObjectData setData = testObjectData.Property("StringProperty", expected);
        
        testObjectData.Property("StringProperty").Decode().ShouldEqual(expected);
        setData.Property("StringProperty").Decode().ShouldEqual(expected);
        setData.ShouldEqual(testObjectData);
        setData.ShouldBe(testObjectData);
    }
}