using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record LobbySettings
{
    public static LobbySettings Default => new()
    {
        PlayersCount = 2, // todo на нужный вариант, пока для теста
        TraitorsCount = 1,
        StartBalance = 100_000,
        RandomizerSeed = Environment.TickCount,
        WelcomeMessage = "Welcome to FGS!",
        GamesSettings = [
            new GameSettingsWrapper
            {
                LobbyGameType = LobbyGameType.Vote,
                VoteGameSettings = new VoteGameSettings
                {
                    CanSkip = false,
                    CanSelfChoice = false,
                    MultiplyChoice = false,
                    MultipleWinner = false,
                    WinnerReward = new WinnerReward
                    {
                        BalanceChangingType = BalanceOperationType.Addition,
                        BalanceChangingValue = 25_000
                    },
                    RandomIndividualVoteGameSettings =
                    [
                    ],
                    GameDescription = "Голосование описание ляляляля"
                }
            }
        ]
    };

    public required uint PlayersCount { get; init; }
    public required uint TraitorsCount { get; init; }
    public required long StartBalance { get; init; }
    public required int RandomizerSeed { get; init; }
    public required string WelcomeMessage { get; init; }
    public required List<GameSettingsWrapper> GamesSettings { get; init; }
}
