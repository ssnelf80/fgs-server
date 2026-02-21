namespace FGS.Domain.FgsLobby.Services.FgsLobbyState;

public interface ILobbyStateRequest;

public record AddPlayerRequest(Guid UserId) : ILobbyStateRequest;
public record RemovePlayerRequest(Guid UserId) : ILobbyStateRequest;
public record InitializeGameRequest : ILobbyStateRequest;