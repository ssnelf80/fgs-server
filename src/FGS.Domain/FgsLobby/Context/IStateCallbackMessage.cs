namespace FGS.Domain.FgsLobby.Context;

public interface IStateCallbackMessage
{
    
}

public record EmptyStateCallbackMessage() : IStateCallbackMessage
{
    public static readonly EmptyStateCallbackMessage Instance = new();
}

public record NotReadyStateCallbackMessage() : IStateCallbackMessage
{
    public static readonly NotReadyStateCallbackMessage Instance = new();
}

