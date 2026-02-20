namespace FGS.Domain.FgsLobby.Events;

public record PlayerDisconnectedLobbyEvent(Guid LobbyId,Guid UserId) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct) => visitor.Visit(this, ct);
}