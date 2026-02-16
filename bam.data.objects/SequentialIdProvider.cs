namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="ISequentialIdProvider"/> that persists the latest sequential ID to a file.
/// </summary>
public class SequentialIdProvider: ISequentialIdProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequentialIdProvider"/> class.
    /// </summary>
    /// <param name="saveLatestTo">The file path to persist the latest sequential ID to.</param>
    public SequentialIdProvider(string saveLatestTo)
    {
        this.SaveLatestTo = saveLatestTo;
    }
    
    protected string SaveLatestTo { get; }
    protected ulong Latest { get; set; }
    
    /// <inheritdoc />
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