using FGS.Domain.FgsLobby.Events;

namespace FGS.Domain.FgsLobby;

public interface ILobbyEventVisitor<out T>
{
    T Visit(LobbyCreatedEvent e);
    T Visit(LobbyStatusChangedEvent e);
    T Visit(PlayerConnectedLobbyEvent e);
    T Visit(PlayerDisconnectedLobbyEvent e);
}