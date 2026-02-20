using FGS.Domain.FgsLobby.Entities;

namespace FGS.Domain.FgsLobby.Events;

public record LobbyCreatedEvent(
    Guid LobbyId,
    string Name,
    Guid MasterUserId,
    LobbySettings LobbySettings,
    DateTimeOffset CreatedAt
) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct) => visitor.Visit(this, ct);
}