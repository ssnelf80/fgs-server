namespace FGS.Domain.FgsLobby.Entities;

public record LobbyStateChangedHubEvent(Guid LobbyId, string EventType);
