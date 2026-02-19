using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Events;

public record LobbyStatusChangedEvent(
    Guid Id,
    LobbyStatus Status
) : LobbyEvent(Id)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor) => visitor.Visit(this);
}