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

    private readonly IReadOnlyList<string> _defaultChoicesMap = 
    [
        Rock,
        Paper,
        Scissors
    ];

    private Dictionary<Guid, IReadOnlyList<string>> UserChoicesHistory { get; init; } = [];
    
    protected override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.RockPaperScissors;
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        throw new NotImplementedException();
    }

    protected override IReadOnlyList<string> GetUserRandomChoices(Guid userId) => [GetRandomItem(GetValidUserChoices(userId))];

    protected override GameStatus SetResult()
    {
        //var currentRoundSettings = GameSettings.GlobalGameSettings.RoundGameSettings[RoundNumber];
        throw new NotImplementedException();
    }

    protected override void RoundSetLocalSettings()
    {
        throw new NotImplementedException();
    }

    protected override ValidationResult Validate(SetUserChoicesRequest request)
    {
        if (!request.Choices.All(choice =>
                GetValidUserChoices(request.UserId).Contains(choice)))
            return ValidationResult.Failure(new InvalidOperationLobbyStateException("Invalid user choice"));
        
        return ValidationResult.Success;
    }

    private IReadOnlyList<string> GetValidUserChoices(Guid userId)
    {
        var result = _defaultChoicesMap.Except(UserChoicesHistory[userId]).ToList();
        var currentUserSettings = UserGameSettingsMap[userId];

        if (currentUserSettings.PaidChoice)
        {
            List<string> paidChoices = [];
            paidChoices.AddRange(
                from casualChoice in result 
                from historyChoice in UserChoicesHistory[userId] 
                select $"{casualChoice}{Delimiter}{historyChoice}");

            result.AddRange(paidChoices);
        }
        
        return result;
    }
}