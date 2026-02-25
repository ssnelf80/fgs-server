namespace FGS.Domain.FgsLobby.Context.GameSettings;

public record VoteGameSettings
{
    public required string GameDescription { get; init; }
    public required bool CanSkip { get; init; }
    public required bool CanSelfChoice { get; init; }
    public required bool MultiplyChoice  { get; init; } 
    public required bool MultipleWinner { get; init; }
    public required WinnerReward WinnerReward { get; init; }
    public required IReadOnlyList<VoteGameSettings> RandomIndividualVoteGameSettings { get; init; }
}