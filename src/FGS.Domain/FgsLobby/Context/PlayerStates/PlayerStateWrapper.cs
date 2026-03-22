using FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.PlayerStates;

// todo нужен глубокий рефактор и переосмысление
public record PlayerStateWrapper
{
    public long Balance { get; init; }
    public required PlayerRole PlayerRole { get; init; }
    public required LobbyGameStateTypeEnum LobbyGameType { get; init; }
    public required int GameNumber { get; init; }
    public required string Message { get; init; }
    public required IGameState GameState {get; init; }
}