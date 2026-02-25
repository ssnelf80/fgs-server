using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record BalanceOperation(BalanceOperationType Type, decimal Value);
