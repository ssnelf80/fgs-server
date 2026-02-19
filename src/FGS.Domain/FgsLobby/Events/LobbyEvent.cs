using FGS.Domain.Base;

namespace FGS.Domain.FgsLobby.Events;

public abstract record LobbyEvent(Guid Id) : IDomainEvent
{
    public abstract T Accept<T>(ILobbyEventVisitor<T> visitor);
}