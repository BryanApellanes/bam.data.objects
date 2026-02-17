using Bam.Console;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectData should")]
public class ObjectDataShould : UnitTestMenuContainer
{
    public ObjectDataShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void HaveType()
    {
        When.A<PlainTestClass>("is wrapped in ObjectData", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = 16.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(100, 1000);
            ptc.DateTimeProperty = DateTime.Now;
            return new ObjectData(ptc);
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .As<ObjectData>("has a TypeDescriptor", od => od?.TypeDescriptor != null)
                .As<ObjectData>("TypeDescriptor.Type is not null", od => od?.TypeDescriptor?.Type != null)
                .As<ObjectData>("TypeDescriptor.Type is PlainTestClass", od => od?.TypeDescriptor?.Type == typeof(PlainTestClass))
                .As<ObjectData>("has an AssemblyQualifiedTypeName", od => od?.TypeDescriptor?.AssemblyQualifiedTypeName != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BeIObjectData()
    {
        When.A<PlainTestClass>("is wrapped in ObjectData", (ptc) => new ObjectData(ptc))
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("is IObjectData", because.Result is IObjectData);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void HaveProperties()
    {
        When.A<PlainTestClass>("is wrapped in ObjectData", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = 16.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(100, 1000);
            ptc.DateTimeProperty = DateTime.Now;
            return new ObjectData(ptc);
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .As<ObjectData>("has Properties", od => od?.Properties != null)
                .As<ObjectData>("has 4 properties", od => od?.Properties?.Count() == 4);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void OutputSameJson()
    {
        string? expected = null;

        When.A<PlainTestClass>("is wrapped in ObjectData and serialized to Json", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = 16.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(100, 1000);
            ptc.DateTimeProperty = DateTime.Now;
            expected = ptc.ToJson();
            return new ObjectData(ptc);
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .As<ObjectData>("outputs same Json", od => expected!.Equals(od?.ToJson()));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetIntPropertyValue()
    {
        int expected = RandomNumber.Between(100, 1000);

        When.A<PlainTestClass>("is wrapped in ObjectData and IntProperty is decoded", (ptc) =>
        {
            ptc.IntProperty = expected;
            ptc.StringProperty = 8.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(100, 1000);
            ptc.DateTimeProperty = DateTime.Now;
            ObjectData testObjectData = new ObjectData(ptc);
            IProperty property = testObjectData.Property("IntProperty")!;
            return property.Decode();
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("decoded value equals expected", Convert.ToInt64(because.Result) == (long)expected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetStringPropertyValue()
    {
        string expected = 32.RandomLetters();

        When.A<PlainTestClass>("is wrapped in ObjectData and StringProperty is decoded", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = expected;
            ptc.LongProperty = RandomNumber.Between(100, 1000);
            ptc.DateTimeProperty = DateTime.Now;
            ObjectData testObjectData = new ObjectData(ptc);
            IProperty property = testObjectData.Property("StringProperty")!;
            return property.Decode();
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .IsEqualTo(expected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SetPropertyValue()
    {
        string expected = 32.RandomLetters();
        bool originalDiffers = false;
        IObjectData? setData = null;

        When.A<PlainTestClass>("has its StringProperty set via ObjectData", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = 6.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(100, 1000);
            ptc.DateTimeProperty = DateTime.Now;
            ObjectData testObjectData = new ObjectData(ptc);
            originalDiffers = !expected.Equals(testObjectData.Property("StringProperty")!.Decode());
            setData = testObjectData.Property("StringProperty", expected);
            return testObjectData;
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("original value was different from expected", originalDiffers);
            because.TheResult.IsNotNull()
                .As<ObjectData>("has updated StringProperty", od => expected.Equals(od?.Property("StringProperty")?.Decode()));
            because.ItsTrue("setData has updated StringProperty", expected.Equals(setData?.Property("StringProperty")?.Decode()));
            because.ItsTrue("setData equals testObjectData", setData?.Equals(because.Result) == true);
            because.ItsTrue("setData is same reference as testObjectData", ReferenceEquals(setData, because.Result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
