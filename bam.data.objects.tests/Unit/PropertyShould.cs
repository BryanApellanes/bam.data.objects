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
        PlainTestClass @class = new PlainTestClass
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 10.RandomLetters(),
            LongProperty = RandomNumber.Between(1, 100),
            DateTimeProperty = DateTime.Now
        };

        IEnumerable<IProperty> properties = @class.ToObjectProperties();
        properties.Count().ShouldEqual(4);
        Message.PrintLine(properties.ToJson(true));
    }
    
    [UnitTest]
    public void ConvertObjectPropertyListToData()
    {
        PlainTestClass @class = new PlainTestClass
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 10.RandomLetters(),
            LongProperty = RandomNumber.Between(1, 100),
            DateTimeProperty = DateTime.Now
        };

        IEnumerable<IProperty> properties = @class.ToObjectProperties();
        PlainTestClass recovered = properties.FromObjectProperties<PlainTestClass>();

        string originalJson = @class.ToJson();
        string recoveredJson = recovered.ToJson();
        recoveredJson.ShouldEqual(originalJson);
    }

    [UnitTest]
    public void Decode()
    {
        PropertyInfo prop = typeof(PlainTestClass).GetProperty("StringProperty");
        string expected = 16.RandomLetters();
        ObjectData data = new ObjectData(new PlainTestClass { StringProperty = expected });
        Property property = new Property(data, prop.Name, expected);
        object actual = property.Decode();
        actual.ShouldEqual(expected);
    }

    [UnitTest]
    public void SetProperty()
    {
        string expected = 32.RandomLetters();
        ObjectData data = new ObjectData(new PlainTestClass { StringProperty = expected });
        Property prop = new Property(data, "StringProperty", expected);

        PlainTestClass plainTestClass = new PlainTestClass();
        plainTestClass.StringProperty.ShouldBeNull();
        prop.SetValue(plainTestClass);
        plainTestClass.StringProperty.ShouldEqual(expected);
    }

    [UnitTest]
    public void HaveParent()
    {
        ObjectData data = new ObjectData(new PlainTestClass { StringProperty = 16.RandomLetters() });
        IProperty property = data.Property("StringProperty");
        property.ShouldNotBeNull();
        property.Parent.ShouldNotBeNull();
        property.Parent.ShouldEqual(data);
    }
}