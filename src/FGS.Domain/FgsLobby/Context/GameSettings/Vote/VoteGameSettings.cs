using System.Text.Json.Serialization;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings.Vote;

public record VoteGameSettings : IGameSettings<VoteGameSettings, PlayerVoteGameSettings>
{
    public static VoteGameSettings Default => new()
    {
        GameDescription = "Голосование блабла",
        CanSkip = false,
        CanSelfChoice = false,
        CanMultiplyChoice = false,
        MultipleWinner = false,
        WinnerReward = new WinnerReward
        {
           BalanceOperation = new BalanceOperation(BalanceOperationType.Addition, 25_000)
        },
        IndividualDescription = null,
        RandomLocalVoteGameSettings = []
    };

    public required string GameDescription { get; init; }
    public required bool CanSkip { get; init; }
    public required bool CanSelfChoice { get; init; }
    public required bool CanMultiplyChoice  { get; init; } 
    public required bool MultipleWinner { get; init; }
    public required WinnerReward WinnerReward { get; init; }
    public required string? IndividualDescription { get; init; } = null!;
    public required IReadOnlyList<PlayerVoteGameSettings> RandomLocalVoteGameSettings { get; init; }
    
    [JsonIgnore]
    public VoteGameSettings GlobalGameSettings => this;

    [JsonIgnore]
    public PlayerVoteGameSettings DefaultPlayerSettings => new()
    {
        Description = null,
        CanSkip = CanSkip,
        CanSelfChoice = CanSelfChoice,
        CanMultiplyChoice = CanMultiplyChoice,
        WinnerReward =  WinnerReward
    };
}