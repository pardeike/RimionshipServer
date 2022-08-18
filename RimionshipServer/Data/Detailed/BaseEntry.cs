namespace RimionshipServer.Data.Detailed;
public class BaseEntry
{
    public BaseEntry()
    {
    }
    
    public BaseEntry(string UId, DateTime Time, int Id = default)
    {
        this.UId       = UId;
        TimeTicks = Time.Ticks;
        this.Id        = Id;
    }
    public string   UId       { get; init; }
    public long     TimeTicks { get; init; }
    public DateTime Time
    {
        get
        {
            return new DateTime(TimeTicks);
        }
    }
    public int      Id        { get; init; }
    public void Deconstruct(out string UId, out DateTime Time, out int Id)
    {
        UId  = this.UId;
        Time = new DateTime(TimeTicks);
        Id   = this.Id;
    }
}