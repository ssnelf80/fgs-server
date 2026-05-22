using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;
using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Context.States.Base;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyRockPaperScissorsState(LobbyState other, RockPaperScissorsSettings gameSettings)
    : LobbyGameBase<RockPaperScissorsSettings, PlayerRockPaperScissorsSettings>(other, gameSettings, false)
{
    private const string Rock = "Rock";
    private const string Paper = "Paper";
    private const string Scissors = "Scissors";
    private const string Delimiter = "|";
    private const int LastRoundNumber = 2;

    private readonly IReadOnlyList<string> _defaultChoicesMap = 
    [
        Rock,
        Paper,
        Scissors
    ];

    private readonly Dictionary<Guid, IReadOnlyList<string>> _userChoicesHistory  = [];
    
    protected override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.RockPaperScissors;
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        throw new NotImplementedException();
    }

    protected override IReadOnlyList<string> GetUserRandomChoices(Guid userId) => [GetRandomItem(GetValidUserChoices(userId))];

    protected override GameStatus SetResult()
    {
        // проверка штрафного выбора
        // проверка выбора за деньги
        foreach (var player in Players())
        {
            if (UserGameSettingsMap[player.UserId].PlayerMode
                is not (RockPaperScissorsPlayerModeEnum.FeeChoice or RockPaperScissorsPlayerModeEnum.PaidChoice))
                continue;
            
            var playerChoice = UserChoicesMap[player.UserId].First();
            if (UserGameSettingsMap[player.UserId].TriggerChoices.Contains(playerChoice))
                ChangeBalance(player.UserId, UserGameSettingsMap[player.UserId].WinnerReward.BalanceOperation);
        }
        
        // базовый расчет победителя
        var roundResult = new Dictionary<string, int>()
        {
            { Rock, UserChoicesMap.Values.Count(x => NormalizeUserChoices(x) == Rock) },
            { Paper, UserChoicesMap.Values.Count(x => NormalizeUserChoices(x) == Paper) },
            { Scissors, UserChoicesMap.Values.Count(x => NormalizeUserChoices(x) == Scissors) }
        };

        foreach (var player in Players())
        {
            var diff = GetWinLooseDiff(
                roundResult,
                NormalizeUserChoices(UserChoicesMap[player.UserId]),
                UserGameSettingsMap[player.UserId].PlayerMode == RockPaperScissorsPlayerModeEnum.LoseAsWin);

            var rewardOperation = UserGameSettingsMap[player.UserId].WinnerReward.BalanceOperation;
            switch (diff)
            {
                case 0:
                    continue;
                case > 0:
                    ChangeBalance(player.UserId, rewardOperation with
                    {
                        Value = diff * rewardOperation.Value
                    });
                    break;
                case < 0:
                    ChangeBalance(player.UserId, rewardOperation.Invert() with
                    {
                        Value = diff * rewardOperation.Value * -1
                    });
                    break;
            }
        }
        
        // вовзрат к игре или же окончание, в зависимости от номера раунда
        return RoundNumber == LastRoundNumber ? GameStatus.LastShowResult : GameStatus.ShowResult;
    }

    protected override void RoundSetLocalSettings()
    {
        if (RoundNumber < 0 || RoundNumber > 2)
            throw new InvalidOperationLobbyStateException("Round number must be between 0 and 2");
        
        var currentRoundSettings = GameSettings.GlobalGameSettings.RoundGameSettings[RoundNumber];
        
        if (currentRoundSettings.FeeChoicePlayerCount > 0)
        {
            foreach (var choice in GetRandomDefaultChoices(currentRoundSettings.FeeChoicePlayerCount))
            {
                SetLocalPlayerSettings(currentRoundSettings.GetFeeChoicePlayerSetting(choice));
            }
        }

        for (var i = 0; i < currentRoundSettings.LoseAsWinPlayerCount; ++i)
            SetLocalPlayerSettings(currentRoundSettings.GetLoseAsWinPlayerSetting());
       
        for (var i = 0; i < currentRoundSettings.PaidChoicePlayerCount; ++i)
        {
            // немного костылей, так как SetLocalPlayerSettings не знает о платных выборах, а мы не знаем Id игрока до инициаилизации
            if (SetLocalPlayerSettings(currentRoundSettings.GetPaidChoicePlayerSetting()) is { } playerId)
            {
                UserGameSettingsMap[playerId] = UserGameSettingsMap[playerId] with
                {
                    TriggerChoices = GetPaidUserChoices(playerId)
                };
            }
        }
    }

    protected override ValidationResult Validate(SetUserChoicesRequest request)
    {
        if (!request.Choices.All(choice =>
                GetValidUserChoices(request.UserId).Contains(choice)))
            return ValidationResult.Failure(new InvalidOperationLobbyStateException("Invalid user choice"));
        
        if (request.Choices.Length != 1)
            return ValidationResult.Failure(new InvalidOperationLobbyStateException("User not support multiply choices"));
        
        return ValidationResult.Success;
    }

    private int GetWinLooseDiff(IReadOnlyDictionary<string, int> roundResult, string userChoice, bool loseAsWin)
    {
        var diff = 0;
        switch (userChoice)
        {
            case Rock:
                diff += roundResult[Scissors] - roundResult[Paper];
                if (loseAsWin)
                    diff += 2 * roundResult[Paper];
                break;
            case Paper:
                diff += roundResult[Rock] - roundResult[Scissors];
                if (loseAsWin)
                    diff += 2 * roundResult[Scissors];
                break;
            case Scissors:
                diff += roundResult[Paper] - roundResult[Rock];
                if (loseAsWin)
                    diff += 2 * roundResult[Rock];
                break;
            default:
                throw new InvalidOperationLobbyStateException($"Invalid user choice {userChoice}");
        }
        return diff;
    }

    private string NormalizeUserChoices(IReadOnlyList<string> userChoices)
    {
        if (userChoices.Count != 1)
            throw new InvalidOperationLobbyStateException("Invalid count of user choices");

        return userChoices[0] switch
        {
            _ when userChoices[0].EndsWith(Rock) => Rock,
            _ when userChoices[0].EndsWith(Paper) => Paper,
            _ when userChoices[0].EndsWith(Scissors) => Scissors,
            _ => throw new InvalidOperationLobbyStateException($"Invalid user choice {userChoices[0]}")
        };
    }

    private IReadOnlyList<string> GetValidUserChoices(Guid userId)
    {
        var result = _defaultChoicesMap.Except(_userChoicesHistory[userId]).ToList();
        var currentUserSettings = UserGameSettingsMap[userId];
        
        if (currentUserSettings.PlayerMode == RockPaperScissorsPlayerModeEnum.FeeChoice)
            result.AddRange(currentUserSettings.TriggerChoices);
        
        return result;
    }

    private IReadOnlyList<string> GetPaidUserChoices(Guid userId) =>
        (from casualChoice in _defaultChoicesMap.Except(_userChoicesHistory[userId])
            from historyChoice in _userChoicesHistory[userId]
            select $"{casualChoice}{Delimiter}{historyChoice}").ToList();

    private IReadOnlyList<string> GetRandomDefaultChoices(int count) =>
        count switch
        {
            > 0 and <= 3 => _defaultChoicesMap.Concat(_defaultChoicesMap).Skip(Random.Next(0, 3)).Take(count).ToList(),
            _ => throw new InvalidOperationLobbyStateException("count should be from 1 to 3")
        };
}
