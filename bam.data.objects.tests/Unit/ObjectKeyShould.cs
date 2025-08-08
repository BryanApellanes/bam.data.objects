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
        ObjectDataKey key1 = new ObjectDataKey()
        {
            Key = key
        };
        ObjectDataKey key2 = new ObjectDataKey()
        {
            Key = key,
        };
        
        key1.Equals(key2).ShouldBeTrue();
    }
    
    [UnitTest]
    public async Task NotBeEqualWithDifferentKeys()
    {
        string id = 8.RandomLetters();
        string key1 = 16.RandomLetters();
        string key2 = 32.RandomLetters();
        ObjectDataKey objKey1 = new ObjectDataKey()
        {
            Key = key1
        };
        ObjectDataKey objKey2 = new ObjectDataKey()
        {
            Key = key2
        };
        
        key1.Equals(key2).ShouldBeFalse();
    }
}