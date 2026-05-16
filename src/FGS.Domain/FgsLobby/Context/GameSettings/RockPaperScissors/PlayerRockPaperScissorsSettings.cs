namespace FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;

public record PlayerRockPaperScissorsSettings
{
    public required string? Description { get; init; }
    public required bool FeeChoice { get; init; } 
    public required bool PaidChoice { get; init; }
    public required bool LoseAsWin { get; init; }
    public required WinnerReward WinnerReward { get; init; }
}