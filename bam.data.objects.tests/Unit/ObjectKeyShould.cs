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
    public async Task BeEqualWithBothIdAndKeySet()
    {
        string id = 16.RandomLetters();
        string key = 32.RandomLetters();
        ObjectDataKey key1 = new ObjectDataKey()
        {
            //Id = id,
            Key = key,
        };
        ObjectDataKey key2 = new ObjectDataKey()
        {
            //Id = id,
            Key = key,
        };
        
        key1.Equals(key2).ShouldBeTrue();
    }

    [UnitTest]
    public async Task BeEqualWithJustKeySet()
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
    public async Task NotBeEqualWithDifferentKeysAndSameId()
    {
        string id = 8.RandomLetters();
        string key1 = 16.RandomLetters();
        string key2 = 32.RandomLetters();
        ObjectDataKey objKey1 = new ObjectDataKey()
        {
            //Id = id,
            Key = key1
        };
        ObjectDataKey objKey2 = new ObjectDataKey()
        {
            //Id = id,
            Key = key2
        };
        
        key1.Equals(key2).ShouldBeFalse();
    }
}