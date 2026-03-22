namespace FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;

public record WaitPlayerConnectionState(uint TotalPlayersCount, IReadOnlyCollection<Guid> ConnectedUserIds) : IGameState
{
    public PlayerGameStateTypeEnum PlayerGameStateTypeEnum => PlayerGameStateTypeEnum.WaitPlayerConnection;
}