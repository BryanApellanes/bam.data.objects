using Bam.Console;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Dynamic.TestClasses;
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

        IEnumerable<ObjectProperty> properties = data.ToObjectProperties();
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

        IEnumerable<ObjectProperty> properties = data.ToObjectProperties();
        TestData recovered = properties.FromObjectProperties<TestData>();

        string originalJson = data.ToJson();
        string recoveredJson = recovered.ToJson();
        recoveredJson.ShouldEqual(originalJson);
    }
}