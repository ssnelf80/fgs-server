namespace FGS.Domain.FgsLobby.Events;

public record BotConnectedLobbyEvent(Guid LobbyId, Guid BotId) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct = default)
        => visitor.Visit(this, ct);
}