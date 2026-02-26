namespace FGS.Auth.Models;

public record UserInfoModelWithRoles
{
    public required Guid Id { get; init; }
    public required string Login { get; init; }
    public required string DisplayName { get; init; }
    public required IList<string> Roles { get; init; }
}