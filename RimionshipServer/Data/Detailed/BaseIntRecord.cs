namespace RimionshipServer.Data.Detailed;

public class BaseIntRecord
{
    public BaseIntRecord()
    {
    }
    
    public BaseIntRecord(BaseEntry Id, int Value)
    {
        this.Id    = Id;
        this.Value = Value;
    }
    public BaseEntry Id    { get; init; }
    public int       Value { get; init; }
}