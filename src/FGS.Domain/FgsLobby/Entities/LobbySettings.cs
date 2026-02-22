namespace FGS.Domain.FgsLobby.Entities;

public record LobbySettings
{
    public static LobbySettings Default => new()
    {
        PlayersCount = 2, // todo на нужный вариант, пока для теста
        TraitorsCount = 1,
        StartBalance = 100_000,
        RandomizerSeed = Environment.TickCount,
    };

    public required uint PlayersCount { get; init; }
    public required uint TraitorsCount { get; init; }
    public required long StartBalance { get; init; }
    public required int RandomizerSeed { get; init; }
}
