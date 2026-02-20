using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.FgsLobby.Services;

namespace FGS.Domain.FgsLobby;

public partial class Lobby
{
    private sealed class InnerLobbyManagerVisitor(Lobby lobby) : ILobbyEventVisitor<bool>
    {
        public bool Visit(LobbyCreatedEvent e, CancellationToken ct = default)
        {
            lobby._lobbyGameManager = new LobbyGameManager(e.LobbySettings);
            return true;
        }

        public bool Visit(LobbyStatusChangedEvent e, CancellationToken ct = default)
        {
            lobby.Status = e.Status;
            return true;
        }

        public bool Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.LobbyGameManager.AddPlayer(e.UserId);
            if (lobby.LobbyGameManager.ReadyToStartInitialize())
            {
                lobby.LobbyGameManager.InitializeStartState();
                lobby.EmitEvent(new LobbyStatusChangedEvent(lobby.Id, LobbyStatus.InProgress));
            }
            
            return true;
        }

        public bool Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.LobbyGameManager.RemovePlayer(e.UserId);
            return true;
        }
    }
}