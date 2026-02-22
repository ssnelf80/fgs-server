using FGS.Domain.FgsLobby.Context;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Context.States;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context;

public class LobbyStateContext
{
    private LobbyState _state = null!;
    public LobbyGameStateEnum Status => _state.GameState;

    protected LobbyStateContext(LobbyState state)
    {
        TransitionTo(state);
    }
    
    public void TransitionTo(LobbyState state)
    {
       _state = state;
       _state.SetContext(this);
    }

    public void SendRequest(ILobbyContextRequest request) => _state.Handle(request);

    public static LobbyStateContext Create(LobbySettings lobbySettings)
    {
        var lobbyState = new LobbyWaitPlayerConnectionState(lobbySettings, []);
        var lobbyContext = new LobbyStateContext(lobbyState);
        return lobbyContext;
    }
}