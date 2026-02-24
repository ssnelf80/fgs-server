namespace FGS.Domain.FgsLobby.Events;

public record RemoveBotFromPlayerLobbyEvent(Guid LobbyId, Guid UserId) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct = default)
        => visitor.Visit(this, ct);
}