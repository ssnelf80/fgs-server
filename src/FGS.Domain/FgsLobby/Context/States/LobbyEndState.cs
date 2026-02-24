using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyEndState(LobbyState other) : LobbyState(other)
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.EndState;
    protected override void DoBotActions()
    {
        throw new NotImplementedException();
    }

    protected override string[] GetRandomPlayerChoice(Guid userId)
    {
        throw new NotImplementedException();
    }
}