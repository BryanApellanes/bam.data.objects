using Bam.Data.Dynamic.TestClasses;
using Bam.Data.Objects;
using Bam.Net.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

[UnitTestMenu("ObjectData should")]
public class ObjectDataShould : UnitTestMenuContainer
{
    public ObjectDataShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
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
        
        data.Type.ShouldNotBeNull("Type was null");
        data.Type.ShouldBe(typeof(TestData));
        data.Type.ShouldNotBeNull("TypeIdentifier was null");
        data.Type.Type.ShouldNotBeNull("TypeIdentifier.Type was null");
        data.Type.AssemblyQualifiedTypeName.ShouldNotBeNull("TypeIdentifier.AssemblyQualifiedTypeName was null");
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
}