namespace FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;

public record PlayerRockPaperScissorsSettings
{
    public RockPaperScissorsPlayerModeEnum PlayerMode { get; init; }
    public required string? Description { get; init; }
    public required WinnerReward WinnerReward { get; init; }
    public required IReadOnlyList<string> TriggerChoices { get; init; }
}