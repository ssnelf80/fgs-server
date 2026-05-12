using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public sealed class LobbyVoteState : LobbyConfirmationBase
{
    private enum VoteStatus
    {
        Vote,
        ShowResult
    }
    private VoteStatus CurrentVoteStatus
    {
        get;
        set
        {
            if (value == VoteStatus.ShowResult)
                IsConfirmationMode = true;

            field = value;
        }
    } = VoteStatus.Vote;
    private const string SkipVariant = "SKIP";
    private readonly VoteGameSettings _globalSettings;
    private readonly Dictionary<Guid, VoteGameSettings> _userVoteGameSettingsMap = [];
    private readonly HashSet<Guid> _usersWithIndividualVoteGameSettings = [];
    private readonly Dictionary<Guid, IReadOnlyCollection<string>> _userChoicesMap = [];

    public LobbyVoteState(LobbyState other, IVoteSettings voteSettings) : base(other, false)
    {
        _globalSettings = voteSettings.VoteGameSettings;
        InitUserGameSettings();
        DoBotActions();
        GoToNextGameIfNeeded();
    }

    public override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.Vote;
    public LobbyGameType GameType => LobbyGameType.Vote;

    public override void Handle(ILobbyContextRequest request)
    {
        if (CurrentVoteStatus == VoteStatus.ShowResult)
        {
            base.Handle(request);
            return;
        }
            
        switch (request)
        {
            case SetUserChoicesRequest userChoiceRequest when !IsPlayerExists(userChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {userChoiceRequest.UserId} is not exist");
            case SetUserChoicesRequest { Choices.Length: 0 } userChoicesRequest:
                    _userChoicesMap.Remove(userChoicesRequest.UserId);
                return;
            case SetUserChoicesRequest userChoicesRequest
                when !userChoicesRequest.Choices.All(choice => GetValidUserChoices(userChoicesRequest.UserId).Contains(choice)):
                throw new InvalidOperationLobbyStateException("Invalid user choice");
            case SetUserChoicesRequest userChoicesRequest
                when !_userVoteGameSettingsMap[userChoicesRequest.UserId].CanMultiplyChoice &&
                     userChoicesRequest.Choices.Length != 1:
                throw new InvalidOperationLobbyStateException("User not support multiply choices");
            case SetUserChoicesRequest userChoicesRequest:
                    _userChoicesMap[userChoicesRequest.UserId] = userChoicesRequest.Choices;
                    SetResultIfNeeded();
                    break;
            case SetRandomUserChoicesRequest randomChoiceRequest when !IsPlayerExists(randomChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {randomChoiceRequest.UserId} is not exist");
            case SetRandomUserChoicesRequest rndUserChoicesRequest:
                _userChoicesMap[rndUserChoicesRequest.UserId] = GetUserRandomChoices(rndUserChoicesRequest.UserId);
                SetResultIfNeeded();
                break;
            default:
                base.Handle(request);
                break;
        }
    }

    protected override void DoBotActions()
    {
        if (IsConfirmationMode)
        {
            base.DoBotActions();
        }
        else
        {
            foreach (var player in BotPlayers())
                _userChoicesMap[player.UserId] = GetUserRandomChoices(player.UserId);
            
            SetResultIfNeeded();
        }

        GoToNextGameIfNeeded();
    }

    private IReadOnlyCollection<string> GetUserSelectedChoices(Guid userId)
    {
        if (_userChoicesMap.TryGetValue(userId, out var choices))
            return choices;
        return [];
    }

    private void SetResultIfNeeded()
    {
        if (!CanSetResult()) 
            return;
        
        SetResult();
        DoBotActions();
    }

    private bool CanSetResult() => CurrentVoteStatus == VoteStatus.Vote && _userChoicesMap.Count == Players().Count;

    private void SetResult()
    {
        if (!CanSetResult())
            throw new InvalidOperationLobbyStateException(
                $"can't set result: {CurrentVoteStatus} and {_userChoicesMap.Count}/{Players().Count}");

        var choicesCount = Players().ToDictionary(x => x.UserId, _ => 0);
        foreach (var choice in _userChoicesMap.Values.SelectMany(x => x))
        {
            if (choice == SkipVariant)
                continue;
            choicesCount[Guid.Parse(choice)]++;
        }

        var maxValue = choicesCount.Values.Max();
        var winnersIds = choicesCount
            .Where(x => x.Value == maxValue)
            .Select(x => x.Key)
            .ToList();
        try
        {
            if (choicesCount[winnersIds[0]] == 0) // у победителя ноль голосов
                return;
            if (!_globalSettings.MultipleWinner && winnersIds.Count > 1) // несколько победителей
                return;
            ApplyWinnerReward(winnersIds);
        }
        finally
        {
            CurrentVoteStatus = VoteStatus.ShowResult;
        }
    }

    private void ApplyWinnerReward(IReadOnlyList<Guid> winnersIds)
    {
        foreach (var winnerId in winnersIds)
            ChangeBalance(winnerId, _userVoteGameSettingsMap[winnerId].WinnerReward.BalanceOperation);
    }

    private void InitUserGameSettings()
    {
        foreach (var player in Players())
            _userVoteGameSettingsMap.Add(player.UserId, _globalSettings);

        foreach (var individualSettings in _globalSettings.RandomIndividualVoteGameSettings)
            SetIndividualVoteGameSettings(individualSettings);
    }

    private void SetIndividualVoteGameSettings(VoteGameSettings settings)
    {
        if (_usersWithIndividualVoteGameSettings.Count == Players().Count) // todo log?
            return;

        var rndPlayer = GetRandomPlayerWithoutIndividualVoteGameSettings();
        _usersWithIndividualVoteGameSettings.Add(rndPlayer.UserId);
        _userVoteGameSettingsMap[rndPlayer.UserId] = settings;
    }

    private Player GetRandomPlayerWithoutIndividualVoteGameSettings()
        => GetRandomPlayer(Players()
            .Where(x => !_usersWithIndividualVoteGameSettings.Contains(x.UserId))
            .OrderBy(x => x.UserId)
            .ToList());

    private IReadOnlyList<string> GetValidUserChoices(Guid userId)
    {
        List<string> variants = [];
        var settings = _userVoteGameSettingsMap[userId];
        if (settings.CanSkip)
            variants.Add(SkipVariant);
        if (settings.CanSelfChoice)
            variants.Add(userId.ToString());

        variants.AddRange(Players()
            .Where(x => x.UserId != userId)
            .Select(x => x.UserId.ToString()));

        return variants;
    }

    private IReadOnlyList<string> GetUserRandomChoices(Guid userId)
    {
        List<string> result = [];
        var variants = GetValidUserChoices(userId);
        if (!_userVoteGameSettingsMap[userId].CanMultiplyChoice)
        {
            result.Add(variants[Random.Next(0, variants.Count)]);
        }
        else // множественный выбор
        {
            var countOfRandomChoices = Random.Next(1, variants.Count);
            for (var i = 0; i < countOfRandomChoices; i++)
            {
                result.Add(variants[Random.Next(0, variants.Count)]);
                variants = variants.Where(x => !result.Contains(x)).ToList();
            }

            if (result.Contains(SkipVariant))
                result = [SkipVariant];
        }

        return result;
    }

    protected override void GoToNextGameIfNeeded()
    {
        if (CurrentVoteStatus == VoteStatus.ShowResult && IsConfirmed())
            Context.TransitionTo(GetNextGameState());
    }
    
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        var individualSettings = _userVoteGameSettingsMap[userId];
      
        return CurrentVoteStatus switch
        {
            VoteStatus.Vote => new PlayerStateWrapper
            {
                Balance = player.Balance,
                PlayerRole = player.Role,
                LobbyGameType = GameState,
                GameNumber = CurrentGameNumber,
                Message = _globalSettings.GameDescription,
                GameState =
                    new VoteState
                    {
                        CanMultiplyChoice = individualSettings.CanMultiplyChoice,
                        CanSelfChoice = individualSettings.CanSelfChoice,
                        EnabledChoices = GetValidUserChoices(userId),
                        SelectedChoices = GetUserSelectedChoices(userId),
                        IndividualDescription = individualSettings.IndividualDescription,
                    }
            },
            VoteStatus.ShowResult => new PlayerStateWrapper
            {
                Balance = player.Balance,
                PlayerRole = player.Role,
                LobbyGameType = GameState,
                GameNumber = CurrentGameNumber,
                Message = _globalSettings.GameDescription,
                GameState =
                    new ConfirmationState(
                        IsPlayerConfirm(userId)) // todo {nazarov} нужен отдельный тип для показа результата
            },
            _ => throw new InvalidOperationLobbyStateException($"Unknown CurrentVoteStatus: {CurrentVoteStatus}")
        };
    }
}