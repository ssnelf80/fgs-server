using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Services.FgsLobbyState;

public class LobbyStateContext
{
    private AbstractLobbyState _state;
    public LobbyGameStateEnum Status => _state.GameState;

    protected LobbyStateContext(AbstractLobbyState state)
    {
        _state = state;
    }
    
    public void TransitionTo(AbstractLobbyState state)
    {
       _state = state;
       _state.SetContext(this);
    }

    public void SendRequest(ILobbyStateRequest request) => _state.Handle(request);

    public static LobbyStateContext Create(LobbySettings lobbySettings) 
        => new(new LobbyWaitPlayerConnectionState(lobbySettings, []));
}