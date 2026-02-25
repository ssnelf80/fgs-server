using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record LobbySettings
{
    public static LobbySettings Default =>
        new()
        {
            PlayersCount = 4,
            TraitorsCount = 1,
            StartBalance = 100_000,
            RandomizerSeed = Environment.TickCount,
            WelcomeMessage = "Welcome to FGS!",
            GamesSettings =
            [
                new GameSettingsWrapper
                {
                    LobbyGameType = LobbyGameType.Vote,
                    VoteGameSettings = new VoteGameSettings
                    {
                        CanSkip = true,
                        CanSelfChoice = false,
                        MultiplyChoice = false,
                        MultipleWinner = false,
                        IndividualDescription = null,
                        WinnerReward = new WinnerReward
                        {
                            BalanceOperation = new BalanceOperation(BalanceOperationType.Addition, 25_000)
                        },
                        RandomIndividualVoteGameSettings =
                        [
                            VoteGameSettings.Default with { MultiplyChoice = true}
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
