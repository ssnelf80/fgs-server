using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record BalanceOperation(BalanceOperationType Type, decimal Value)
{
    public BalanceOperation Invert() => this with { Type = Type switch
    {
        BalanceOperationType.Addition => BalanceOperationType.Subtraction,
        BalanceOperationType.Subtraction => BalanceOperationType.Addition,
        BalanceOperationType.Multiplication => BalanceOperationType.Division,
        BalanceOperationType.Division => BalanceOperationType.Multiplication,
        BalanceOperationType.NoAction => BalanceOperationType.NoAction,
        _ => throw new ArgumentOutOfRangeException()
    } };
}
