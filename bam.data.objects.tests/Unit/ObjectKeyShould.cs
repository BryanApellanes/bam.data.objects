using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("ObjectKey should")]
public class ObjectKeyShould : UnitTestMenuContainer
{
    public ObjectKeyShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task BeEqualWithKeySet()
    {
        string key = 16.RandomLetters();

        When.A<ObjectDataKey>("is compared to another with the same key",
            () => new ObjectDataKey { Key = key },
            (key1) =>
            {
                ObjectDataKey key2 = new ObjectDataKey { Key = key };
                return key1.Equals(key2);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("keys are equal", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task NotBeEqualWithDifferentKeys()
    {
        string keyValue1 = 16.RandomLetters();
        string keyValue2 = 32.RandomLetters();

        When.A<ObjectDataKey>("is compared to another with a different key",
            () => new ObjectDataKey { Key = keyValue1 },
            (key1) =>
            {
                ObjectDataKey key2 = new ObjectDataKey { Key = keyValue2 };
                return keyValue1.Equals(keyValue2);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsFalse("keys are not equal", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
