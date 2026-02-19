using FGS.Domain.FgsLobby.Entities;

namespace FGS.Domain.FgsLobby.Events;

public record LobbyCreatedEvent(
    Guid Id,
    string Name,
    Guid MasterUserId,
    LobbySettings LobbySettings,
    DateTimeOffset CreatedAt
) : LobbyEvent(Id)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor) => visitor.Visit(this);
}