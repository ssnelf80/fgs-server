using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings;

public record VoteGameSettings
{
    public static VoteGameSettings Default => new VoteGameSettings
    {
        GameDescription = "Голосование блабла",
        CanSkip = false,
        CanSelfChoice = false,
        MultiplyChoice = false,
        MultipleWinner = false,
        WinnerReward = new WinnerReward
        {
            BalanceChangingType = BalanceOperationType.Addition,
            BalanceChangingValue = 25_000
        },
        IndividualDescription = null,
        RandomIndividualVoteGameSettings = []
    };

    public required string GameDescription { get; init; }
    public required bool CanSkip { get; init; }
    public required bool CanSelfChoice { get; init; }
    public required bool MultiplyChoice  { get; init; } 
    public required bool MultipleWinner { get; init; }
    public required WinnerReward WinnerReward { get; init; }
    public required string? IndividualDescription { get; init; } = null!;
    public required IReadOnlyList<VoteGameSettings> RandomIndividualVoteGameSettings { get; init; } // todo отдельную сущность
}