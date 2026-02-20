namespace FGS.Domain.FgsLobby.Events;

public record PlayerConnectedLobbyEvent(Guid LobbyId, Guid UserId) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct) => visitor.Visit(this, ct);
}