namespace RimionshipServer.Data.Detailed;

public class BaseFloatRecord
{
    public BaseFloatRecord()
    {
    }
    
    public BaseFloatRecord(BaseEntry Id, float Value)
    {
        this.Id    = Id;
        this.Value = Value;
    }
    public BaseEntry Id    { get; init; }
    public float     Value { get; init; }
}