using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyWelcomeState(LobbyState other) : LobbyState(other)
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.GameWelcomeInformation;
}