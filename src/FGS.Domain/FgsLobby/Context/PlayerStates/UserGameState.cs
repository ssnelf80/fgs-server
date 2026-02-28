using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.PlayerStates;

// todo нужен глубокий рефактор и переосмысление
public record PlayerGameState
{
    public long Balance { get; init; }
    public required PlayerRole PlayerRole { get; init; }
    public required LobbyGameStateEnum GameState { get; init; }
    public required string InnerGameState { get; init; }
    public required int GameNumber { get; init; }
    public required IReadOnlyCollection<string> Choices { get; init; }
    public required IReadOnlyCollection<string> SelectedChoices { get; init; }
    public required bool CanSendChoice { get; init; }
    public required string GameInfoMessage { get; init; }
    public required string RoundInfoMessage { get; init; }
    // todo комментарии к выборам
    // todo возможность мультиселекта
}