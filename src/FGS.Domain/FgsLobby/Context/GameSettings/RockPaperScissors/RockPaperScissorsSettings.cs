using System.Text.Json.Serialization;

namespace FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;

public record RockPaperScissorsSettings : IGameSettings<RockPaperScissorsSettings, PlayerRockPaperScissorsSettings>
{
    public required string GameDescription { get; init; }
    public required WinnerReward WinnerReward { get; init; }
    
    public required IReadOnlyList<RockPaperScissorsRoundSettings> RoundGameSettings { get; init; } = [];
    
    [JsonIgnore]
    public RockPaperScissorsSettings GlobalGameSettings => this;

    [JsonIgnore]
    public PlayerRockPaperScissorsSettings DefaultPlayerSettings => new()
    {
        Description = null,
        PlayerMode = RockPaperScissorsPlayerModeEnum.Default,
        WinnerReward = WinnerReward,
        TriggerChoices = []
    };
}