using FGS.Domain.FgsLobby.Context;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Context.States;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Services.FgsLobbyState;

public class LobbyStateContext
{
    private LobbyState _state;
    public LobbyGameStateEnum Status => _state.GameState;

    protected LobbyStateContext(LobbyState state)
    {
        _state = state;
    }
    
    public void TransitionTo(LobbyState state)
    {
       _state = state;
       _state.SetContext(this);
    }

    public void SendRequest(ILobbyContextRequest request) => _state.Handle(request);

    public static LobbyStateContext Create(LobbySettings lobbySettings) 
        => new(new LobbyWaitPlayerConnectionState(lobbySettings, []));
}