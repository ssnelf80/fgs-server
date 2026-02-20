namespace FGS.Domain.FgsLobby.Entities;

public record LobbySettings
{
    public required uint PlayersCount { get; init; }
    public required Guid MasterUserId { get; init; }
    public required uint TraitorsCount { get; init; }
    public required long StartBalance { get; init; }
    public required int RandomizerSeed { get; init; }
}
