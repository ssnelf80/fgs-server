namespace FGS.Domain.FgsLobby.Context.GameSettings.Vote;

public record PlayerVoteGameSettings
{
    public required string? Description { get; init; }
    public required bool CanSkip { get; init; }
    public required bool CanSelfChoice { get; init; }
    public required bool CanMultiplyChoice  { get; init; }
    public required WinnerReward WinnerReward { get; init; }
}