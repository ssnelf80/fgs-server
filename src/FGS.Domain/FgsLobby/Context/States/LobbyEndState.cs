using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyEndState(LobbyState other) : LobbyState(other)
{
    public override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.End;
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        return new PlayerStateWrapper
        {
            Balance = player.Balance,
            PlayerRole = player.Role,
            LobbyGameType = GameState,
            GameState = EmptyState.Instance,
            GameNumber = CurrentGameNumber,
            Message = "Игра завершена"
        };
    }
}