namespace RimionshipServer.Data
{
    public record UserEvent(string UserId, int Ticks, string Name, string Quest, string Faction, float Points, string Strategy, string ArrivalMode);
}
