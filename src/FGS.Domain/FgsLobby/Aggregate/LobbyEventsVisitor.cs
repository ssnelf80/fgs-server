using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Events;

namespace FGS.Domain.FgsLobby.Aggregate;

public sealed partial class Lobby
{
    // todo подумать над отказом
    private sealed class InnerLobbyManagerVisitor(Lobby lobby) : ILobbyEventVisitor<bool>
    {
        public bool Visit(LobbyCreatedEvent e, CancellationToken ct = default)
        {
            lobby.InitContext(e.LobbySettings);
            return true;
        }

        public bool Visit(LobbyStatusChangedEvent e, CancellationToken ct = default)
        {
            lobby.Status = e.Status;
            return true;
        }

        public bool Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new AddPlayerRequest(e.UserId));
            return true;
        }

        public bool Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new RemovePlayerRequest(e.UserId));
            return true;
        }
    }
}