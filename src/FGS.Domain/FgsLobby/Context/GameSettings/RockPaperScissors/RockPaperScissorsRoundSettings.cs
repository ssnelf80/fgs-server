using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;

public record RockPaperScissorsRoundSettings
{
    public static RockPaperScissorsRoundSettings Default = new()
    {
        FeeChoicePlayerCount = 0,
        PaidChoicePlayerCount = 0,
        LoseAsWinPlayerCount = 0,
        FeeCost = new BalanceOperation(BalanceOperationType.Addition, 0),
        PaidCost =  new BalanceOperation(BalanceOperationType.Addition, 0),
        LoseAsWinCost = new BalanceOperation(BalanceOperationType.Addition, 0)
    };

    public required int FeeChoicePlayerCount { get; init; }
    public required int PaidChoicePlayerCount { get; init; }
    public required int LoseAsWinPlayerCount { get; init; }
    
    public required BalanceOperation FeeCost { get; init; }
    public required BalanceOperation PaidCost { get; init; }
    public required BalanceOperation LoseAsWinCost { get; init; }
}