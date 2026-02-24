namespace FGS.Domain.FgsLobby.Events;

public record LobbySetUserChoiceEvent(Guid LobbyId, Guid UserId, params string[] Choices) : LobbyEvent(LobbyId)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor, CancellationToken ct = default) => visitor.Visit(this, ct);
}