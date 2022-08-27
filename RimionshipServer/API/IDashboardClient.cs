namespace RimionshipServer.API
{
    public record UserInfo(string Id, string UserName, string? AvatarUrl);
    public record DirectionInstruction(string UserId, string? Comment);

    public interface IDashboardClient
    {
        Task AddUser(UserInfo userInfo);
        Task SetDirectionInstruction(DirectionInstruction instruction);
    }
}
