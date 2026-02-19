namespace FGS.Domain.FgsLobby.Events;

public record PlayerConnectedLobbyEvent(Guid Id, Guid UserId) : LobbyEvent(Id)
{
    public override T Accept<T>(ILobbyEventVisitor<T> visitor) => visitor.Visit(this);
}