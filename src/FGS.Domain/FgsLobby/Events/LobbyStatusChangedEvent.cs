using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Events;

public record LobbyStatusChangedEvent(
    Guid LobbyId,
    LobbyStatus Status
) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct) => visitor.Visit(this, ct);
}