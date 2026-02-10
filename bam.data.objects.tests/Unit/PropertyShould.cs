using System.Reflection;
using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("PropertyShould", "ops")]
public class PropertyShould
{
    [UnitTest]
    public void ConvertDataToObjectPropertyList()
    {
        When.A<PlainTestClass>("is converted to object properties", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = 10.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(1, 100);
            ptc.DateTimeProperty = DateTime.Now;
            return ptc.ToObjectProperties();
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .As<IEnumerable<IProperty>>("has 4 properties", props => props?.Count() == 4);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ConvertObjectPropertyListToData()
    {
        string originalJson = null;

        When.A<PlainTestClass>("is round-tripped through object properties", (ptc) =>
        {
            ptc.IntProperty = RandomNumber.Between(1, 100);
            ptc.StringProperty = 10.RandomLetters();
            ptc.LongProperty = RandomNumber.Between(1, 100);
            ptc.DateTimeProperty = DateTime.Now;
            originalJson = ptc.ToJson();
            IEnumerable<IProperty> properties = ptc.ToObjectProperties();
            PlainTestClass recovered = properties.FromObjectProperties<PlainTestClass>();
            return recovered.ToJson();
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull()
                .IsEqualTo(originalJson);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void Decode()
    {
        string expected = 16.RandomLetters();

        When.A<PlainTestClass>("has a Property that is decoded", (ptc) =>
        {
            ptc.StringProperty = expected;
            ObjectData data = new ObjectData(ptc);
            PropertyInfo prop = typeof(PlainTestClass).GetProperty("StringProperty");
            Property property = new Property(data, prop.Name, expected);
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
    public void SetProperty()
    {
        string expected = 32.RandomLetters();
        PlainTestClass target = new PlainTestClass();

        When.A<PlainTestClass>("has a Property set on a target", (ptc) =>
        {
            ptc.StringProperty = expected;
            ObjectData data = new ObjectData(ptc);
            Property prop = new Property(data, "StringProperty", expected);
            prop.SetValue(target);
            return target.StringProperty;
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("target StringProperty was initially null", target != null);
            because.TheResult.IsNotNull()
                .IsEqualTo(expected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void HaveParent()
    {
        When.A<PlainTestClass>("is wrapped in ObjectData and a property is retrieved", (ptc) =>
        {
            ptc.StringProperty = 16.RandomLetters();
            ObjectData data = new ObjectData(ptc);
            return new object[] { data, data.Property("StringProperty") };
        })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            ObjectData data = (ObjectData)results[0];
            IProperty property = (IProperty)results[1];
            because.ItsTrue("property is not null", property != null);
            because.ItsTrue("property has a Parent", property?.Parent != null);
            because.ItsTrue("property Parent is the ObjectData", property?.Parent?.Equals(data) == true);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
