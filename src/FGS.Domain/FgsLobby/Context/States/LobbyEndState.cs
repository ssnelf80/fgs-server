using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyEndState(LobbyState other) : LobbyState(other)
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.EndState;
}