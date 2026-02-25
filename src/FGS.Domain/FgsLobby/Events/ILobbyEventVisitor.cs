namespace FGS.Domain.FgsLobby.Events;

public interface ILobbyEventVisitor<out T>
{
    T Visit(LobbyCreatedEvent e, CancellationToken ct = default);
    T Visit(LobbyStatusChangedEvent e, CancellationToken ct = default);
    T Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default);
    T Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default);
    T Visit(LobbySetUserChoiceEvent e, CancellationToken ct = default);
    T Visit(LobbySetUserRandomChoiceEvent e, CancellationToken ct = default);
    T Visit(SetBotToPlayerLobbyEvent e, CancellationToken ct = default);
    T Visit(RemoveBotFromPlayerLobbyEvent e, CancellationToken ct = default);
}