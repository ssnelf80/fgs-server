using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Context.GameSettings.Vote;
using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Context.States.Base;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States.Vote;

public class LobbyVoteState(LobbyState other, VoteGameSettings gameSettings, bool isChildState)
    : LobbyGameBase<VoteGameSettings, PlayerVoteGameSettings>(other, gameSettings, isChildState)
{
    private const string SkipVariant = "SKIP";
    private VoteStateCallbackMessage? _callbackMessage = null;

    protected override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.Vote;

    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        var individualSettings = UserGameSettingsMap[userId];

        return CurrentGameStatus switch
        {
            GameStatus.InGame => new PlayerStateWrapper
            {
                Balance = player.Balance,
                PlayerRole = player.Role,
                LobbyGameType = GameState,
                GameNumber = CurrentGameNumber,
                Message = GameSettings.GlobalGameSettings.GameDescription,
                GameState =
                    new VoteState
                    {
                        CanMultiplyChoice = individualSettings.CanMultiplyChoice,
                        CanSelfChoice = individualSettings.CanSelfChoice,
                        EnabledChoices = GetValidUserChoices(userId),
                        SelectedChoices = GetUserSelectedChoices(userId),
                        IndividualDescription = individualSettings.Description,
                    }
            },
            GameStatus.LastShowResult => new PlayerStateWrapper
            {
                Balance = player.Balance,
                PlayerRole = player.Role,
                LobbyGameType = GameState,
                GameNumber = CurrentGameNumber,
                Message = GameSettings.GlobalGameSettings.GameDescription,
                GameState = // todo {nazarov} v
                    new ConfirmationState(
                        IsPlayerConfirm(userId)) // todo {nazarov} нужен отдельный тип для показа результата
            },
            _ => throw new InvalidOperationLobbyStateException($"Unknown CurrentVoteStatus: {CurrentGameStatus}")
        };
    }

    protected override IReadOnlyList<string> GetUserRandomChoices(Guid userId)
    {
        List<string> result = [];
        var variants = GetValidUserChoices(userId);
        if (!UserGameSettingsMap[userId].CanMultiplyChoice)
        {
            result.Add(GetRandomItem(variants));
        }
        else // множественный выбор
        {
            var countOfRandomChoices = Random.Next(1, variants.Count);
            for (var i = 0; i < countOfRandomChoices; i++)
            {
                result.Add(GetRandomItem(variants));
                variants = variants.Where(x => !result.Contains(x)).ToList();
            }

            if (result.Contains(SkipVariant))
                result = [SkipVariant];
        }

        return result;
    }

    protected override GameStatus SetResult()
    {
        var choicesCount = Players().ToDictionary(x => x.UserId, _ => 0);
        foreach (var choice in UserChoicesMap.Values.SelectMany(x => x))
        {
            if (choice == SkipVariant)
                continue;
            choicesCount[Guid.Parse(choice)]++;
        }

        _callbackMessage = new VoteStateCallbackMessage(choicesCount);

        var maxValue = choicesCount.Values.Max();
        var winnersIds = choicesCount
            .Where(x => x.Value == maxValue)
            .Select(x => x.Key)
            .ToList();

        if (choicesCount[winnersIds[0]] == 0) // у победителя ноль голосов
            return GameStatus.LastShowResult;
        if (!GameSettings.GlobalGameSettings.MultipleWinner && winnersIds.Count > 1) // несколько победителей
            return GameStatus.LastShowResult;
        
        ApplyWinnerReward(winnersIds);
        return GameStatus.LastShowResult;
    }

    protected override void RoundSetLocalSettings()
    {
        foreach (var localVoteGameSetting in GameSettings.GlobalGameSettings.RandomLocalVoteGameSettings)
            SetLocalPlayerSettings(localVoteGameSetting);
    }

    protected override ValidationResult Validate(SetUserChoicesRequest request)
    {
        if (!request.Choices.All(choice =>
                GetValidUserChoices(request.UserId).Contains(choice)))
            return ValidationResult.Failure(new InvalidOperationLobbyStateException("Invalid user choice"));
        
        if (!UserGameSettingsMap[request.UserId].CanMultiplyChoice && request.Choices.Length != 1)
            return ValidationResult.Failure(new InvalidOperationLobbyStateException("User not support multiply choices"));
        
        return ValidationResult.Success;
    }

    private void ApplyWinnerReward(IReadOnlyList<Guid> winnersIds)
    {
        foreach (var winnerId in winnersIds)
            ChangeBalance(winnerId, UserGameSettingsMap[winnerId].WinnerReward.BalanceOperation);
    }

    private IReadOnlyList<string> GetValidUserChoices(Guid userId)
    {
        List<string> variants = [];
        var settings = UserGameSettingsMap[userId];
        if (settings.CanSkip)
            variants.Add(SkipVariant);
        if (settings.CanSelfChoice)
            variants.Add(userId.ToString());

        variants.AddRange(Players()
            .Where(x => x.UserId != userId)
            .Select(x => x.UserId.ToString()));

        return variants;
    }

    protected override IStateCallbackMessage GetLobbyCallbackMessage() 
        => _callbackMessage is null ? NotReadyStateCallbackMessage.Instance : _callbackMessage;
}