namespace Bam.Data.Objects;

public class SequentialIdProvider: ISequentialIdProvider
{
    public SequentialIdProvider(string saveLatestTo)
    {
        this.SaveLatestTo = saveLatestTo;
    }
    
    protected string SaveLatestTo { get; }
    protected ulong Latest { get; set; }
    
    public ulong GetNextSequentialULong()
    {
        File.WriteAllText(this.SaveLatestTo, (++this.Latest).ToString());
        return this.Latest;
    }

    private void Initialize()
    {
        if (File.Exists(this.SaveLatestTo))
        {
            string content = File.ReadAllText(this.SaveLatestTo);
            ulong latest = 0;
            ulong.TryParse(content, out latest);
            this.Latest = latest;
        }
    }
}