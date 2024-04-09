using System.Reflection;
using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectPropertyShould", "ops")]
public class ObjectPropertyShould
{
    [UnitTest]
    public void ConvertDataToObjectPropertyList()
    {
        TestData data = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 10.RandomLetters(),
            LongProperty = RandomNumber.Between(1, 100),
            DateTimeProperty = DateTime.Now
        };

        IEnumerable<IObjectProperty> properties = data.ToObjectProperties();
        properties.Count().ShouldEqual(4);
        Message.PrintLine(properties.ToJson(true));
    }
    
    [UnitTest]
    public void ConvertObjectPropertyListToData()
    {
        TestData data = new TestData
        {
            IntProperty = RandomNumber.Between(1, 100),
            StringProperty = 10.RandomLetters(),
            LongProperty = RandomNumber.Between(1, 100),
            DateTimeProperty = DateTime.Now
        };

        IEnumerable<IObjectProperty> properties = data.ToObjectProperties();
        TestData recovered = properties.FromObjectProperties<TestData>();

        string originalJson = data.ToJson();
        string recoveredJson = recovered.ToJson();
        recoveredJson.ShouldEqual(originalJson);
    }

    [UnitTest]
    public void Decode()
    {
        PropertyInfo prop = typeof(TestData).GetProperty("StringProperty");
        string expected = 16.RandomLetters();
        ObjectData data = new ObjectData(new TestData { StringProperty = expected });
        ObjectProperty property = new ObjectProperty(data, prop.Name, expected);
        object actual = property.Decode();
        actual.ShouldEqual(expected);
    }

    [UnitTest]
    public void SetProperty()
    {
        string expected = 32.RandomLetters();
        ObjectData data = new ObjectData(new TestData { StringProperty = expected });
        ObjectProperty prop = new ObjectProperty(data, "StringProperty", expected);

        TestData testData = new TestData();
        testData.StringProperty.ShouldBeNull();
        prop.SetValue(testData);
        testData.StringProperty.ShouldEqual(expected);
    }
}