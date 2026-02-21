namespace FGS.Domain.FgsLobby.Context.Requests;

public interface ILobbyContextRequest;

public record AddPlayerRequest(Guid UserId) : ILobbyContextRequest;
public record RemovePlayerRequest(Guid UserId) : ILobbyContextRequest;
public record InitializeGameRequest : ILobbyContextRequest;

public record SendUserValueRequest(Guid UserId, params string[] Values) : ILobbyContextRequest;