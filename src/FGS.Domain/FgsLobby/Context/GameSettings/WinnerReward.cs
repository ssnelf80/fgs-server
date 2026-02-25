using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings;

public record WinnerReward
{
    public required BalanceOperationType BalanceChangingType { get; init; }
    public required long BalanceChangingValue { get; init; }
    // todo дополнительные возможности
}