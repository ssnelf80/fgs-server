using FGS.Domain.Base;

namespace FGS.Domain.FgsLobby.Events;

public abstract record LobbyEvent(Guid LobbyId) : IDomainEvent
{
    public abstract T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct = default);
}