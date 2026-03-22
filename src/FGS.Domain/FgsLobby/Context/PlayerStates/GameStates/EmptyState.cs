namespace FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;

public record EmptyState : IGameState
{
    public static EmptyState Instance = new();
    public PlayerGameStateTypeEnum PlayerGameStateTypeEnum => PlayerGameStateTypeEnum.Empty;
}