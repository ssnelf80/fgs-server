namespace FGS.Domain.FgsLobby.Context.Requests;

public interface ILobbyContextRequest;

public record AddPlayerRequest(Guid UserId, bool IsBot = false) : ILobbyContextRequest;
public record RemovePlayerRequest(Guid UserId) : ILobbyContextRequest;
public record InitializeGameRequest : ILobbyContextRequest;
public record SetUserChoicesRequest(Guid UserId, params string[] Choices) : ILobbyContextRequest;
public record SetRandomUserChoicesRequest(Guid UserId) : ILobbyContextRequest;
public record SetBotToPlayerRequest(Guid UserId) : ILobbyContextRequest;
public record RemoveBotFromPlayerRequest(Guid UserId) : ILobbyContextRequest;

