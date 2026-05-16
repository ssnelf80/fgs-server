namespace FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;

public record RockPaperScissorsRoundSettings
{
    public static RockPaperScissorsRoundSettings Default = new()
    {
        FeeChoicePlayerCount = 0,
        PaidChoicePlayerCount = 0,
        LoseAsWinPlayerCount = 0
    };

    public required int FeeChoicePlayerCount { get; init; }
    public required int PaidChoicePlayerCount { get; init; }
    public required int LoseAsWinPlayerCount { get; init; }
}