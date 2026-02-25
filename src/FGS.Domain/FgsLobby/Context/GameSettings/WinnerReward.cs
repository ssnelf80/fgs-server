using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings;

public record WinnerReward
{
    public required BalanceOperation BalanceOperation { get; init; }
}