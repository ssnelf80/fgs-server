using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyEndState(LobbyState other) : LobbyState(other)
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.EndState;
    public override PlayerGameState GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        return new PlayerGameState
        {
            Balance = player.Balance,
            PlayerRole = player.Role,
            GameState = GameState,
            InnerGameState = string.Empty,
            GameNumber = CurrentGameNumber,
            Choices = [],
            SelectedChoices = [],
            CanSendChoice = false,
            GameInfoMessage = "Игра завершена",
            RoundInfoMessage = string.Empty
        };
    }
}