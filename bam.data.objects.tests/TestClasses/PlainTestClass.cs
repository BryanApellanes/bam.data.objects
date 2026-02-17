namespace Bam.Data.Dynamic.TestClasses;

public class PlainTestClass
{
    public PlainTestClass()
    {
    }

    public PlainTestClass(bool init)
    {
        if (init)
        {
            IntProperty = RandomNumber.Between(100, 5000);
            StringProperty = 32.RandomLetters();
            LongProperty = RandomNumber.Between(10000, 50000);
            DateTimeProperty = DateTime.Now;
        }
    }
    
    public int IntProperty { get; set; }
    public string StringProperty { get; set; } = null!;
    public long LongProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
}