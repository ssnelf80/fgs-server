using System.ComponentModel.DataAnnotations;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;

public record RockPaperScissorsRoundSettings
{
    public static RockPaperScissorsRoundSettings Default = new()
    {
        FeeChoicePlayerCount = 0,
        PaidChoicePlayerCount = 0,
        LoseAsWinPlayerCount = 0,
        FeeCost = new BalanceOperation(BalanceOperationType.Addition, 0),
        PaidCost =  new BalanceOperation(BalanceOperationType.Addition, 0),
        LoseAsWinCost = new BalanceOperation(BalanceOperationType.Addition, 0)
    };

    [Range(0, 2)]
    public required int FeeChoicePlayerCount { get; init; } // todo валидации (больше нуля)
    public required int PaidChoicePlayerCount { get; init; }
    public required int LoseAsWinPlayerCount { get; init; }
    
    public required BalanceOperation FeeCost { get; init; }
    public required BalanceOperation PaidCost { get; init; }
    public required BalanceOperation LoseAsWinCost { get; init; }

    public PlayerRockPaperScissorsSettings GetFeeChoicePlayerSetting(params string[] triggerChoices) =>
        new()
        {
            PlayerMode = RockPaperScissorsPlayerModeEnum.FeeChoice,
            Description = "Вы получите штраф, если выберите выделенный вариант", // todo подумать над текстом
            WinnerReward = new WinnerReward
            {
                BalanceOperation = FeeCost
            },
            TriggerChoices = triggerChoices,
        };
    
    public PlayerRockPaperScissorsSettings GetPaidChoicePlayerSetting(params string[] triggerChoices) =>
        new()
        {
            PlayerMode = RockPaperScissorsPlayerModeEnum.PaidChoice,
            Description = "За отдельную плату вы можете выбрать уже использованные варианты",
            WinnerReward = new WinnerReward
            {
                BalanceOperation = PaidCost
            },
            TriggerChoices = triggerChoices,
        };
    
    public PlayerRockPaperScissorsSettings GetLoseAsWinPlayerSetting(params string[] triggerChoices) =>
        new()
        {
            PlayerMode = RockPaperScissorsPlayerModeEnum.LoseAsWin,
            Description = "При поражении в раунде вы получите награду",
            WinnerReward = new WinnerReward
            {
                BalanceOperation = LoseAsWinCost
            },
            TriggerChoices = triggerChoices
        };
}