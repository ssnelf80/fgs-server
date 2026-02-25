using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public sealed class LobbyVoteState : LobbyState
{
    public enum VoteStatus
    {
        Vote,
        ShowResult
    }

    private VoteStatus _currentVoteStatus = VoteStatus.Vote;
    private const string SkipVariant = "SKIP";
    private readonly VoteGameSettings _globalSettings;
    private Dictionary<Guid, VoteGameSettings> _userVoteGameSettingsMap = [];
    private readonly HashSet<Guid> _usersWithIndividualVoteGameSettings = [];
    private readonly HashSet<Guid> _playerConfirmations = [];
    private readonly Dictionary<Guid, IReadOnlyCollection<string>> _userChoicesMap = [];

    public LobbyVoteState(LobbyState other, IVoteSettings voteSettings) : base(other)
    {
        _globalSettings = voteSettings.VoteGameSettings;
        InitUserGameSettings(voteSettings.VoteGameSettings);
        DoBotActions();
        GoToNextGameIfNeeded();
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.InGame;
    public LobbyGameType GameType => LobbyGameType.Vote;

    public override void Handle(ILobbyContextRequest request)
    {
        switch (request)
        {
            case SetUserChoicesRequest userChoiceRequest when !IsPlayerExists(userChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {userChoiceRequest.UserId} is not exist");
            case SetUserChoicesRequest { Choices.Length: 0 } userChoicesRequest:
                if (_currentVoteStatus == VoteStatus.Vote)
                    _userChoicesMap.Remove(userChoicesRequest.UserId);
                else
                    _playerConfirmations.Remove(userChoicesRequest.UserId);
                return;
            case SetUserChoicesRequest userChoicesRequest
                when !userChoicesRequest.Choices.All(x => GetUserChoices(userChoicesRequest.UserId).Contains(x)):
                throw new InvalidOperationLobbyStateException("Invalid user choice");
            case SetUserChoicesRequest userChoicesRequest
                when !_userVoteGameSettingsMap[userChoicesRequest.UserId].MultiplyChoice &&
                     userChoicesRequest.Choices.Length != 1:
                throw new InvalidOperationLobbyStateException("User not support multiply choices");
            case SetUserChoicesRequest userChoicesRequest:
                if (_currentVoteStatus == VoteStatus.Vote)
                {
                    _userChoicesMap[userChoicesRequest.UserId] = userChoicesRequest.Choices;
                    if (CanSetResult())
                    {
                        SetResult();
                        DoBotActions();
                    }
                 
                }
                else
                {
                    _playerConfirmations.Add(userChoicesRequest.UserId);
                    GoToNextGameIfNeeded();
                }
                break;
            case SetRandomUserChoicesRequest randomChoiceRequest when !IsPlayerExists(randomChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {randomChoiceRequest.UserId} is not exist");
            case SetRandomUserChoicesRequest rndUserChoicesRequest:
                if (_currentVoteStatus == VoteStatus.Vote)
                {
                    _userChoicesMap[rndUserChoicesRequest.UserId] = GetUserRandomChoices(rndUserChoicesRequest.UserId);
                    if (CanSetResult())
                    {
                        SetResult();
                        DoBotActions();
                    }
                }
                else
                {
                    _playerConfirmations.Add(rndUserChoicesRequest.UserId);
                    GoToNextGameIfNeeded();
                }
                break;
            default:
                base.Handle(request);
                break;
        }
    }

    protected override void DoBotActions()
    {
        foreach (var player in BotPlayers())
        {
            if (_currentVoteStatus == VoteStatus.Vote)
            {
                _userChoicesMap[player.UserId] = GetUserRandomChoices(player.UserId);
            }
            else if (_currentVoteStatus == VoteStatus.ShowResult)
            {
                _playerConfirmations.Add(player.UserId);
            }
        }

        if (CanSetResult())
        {
            SetResult();
            DoBotActions();
        }

        GoToNextGameIfNeeded();
    }

    private bool CanSetResult() => _currentVoteStatus == VoteStatus.Vote && _userChoicesMap.Count == Players().Count;

    private void SetResult()
    {
        if (!CanSetResult())
            throw new InvalidOperationLobbyStateException(
                $"can't set result: {_currentVoteStatus} and {_userChoicesMap.Count}/{Players().Count}");

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
            _currentVoteStatus = VoteStatus.ShowResult;
        }
    }

    private void ApplyWinnerReward(IReadOnlyList<Guid> winnersIds)
    {
        foreach (var winnerId in winnersIds)
            ChangeBalance(winnerId, _userVoteGameSettingsMap[winnerId].WinnerReward.BalanceOperation);
    }

    private void InitUserGameSettings(VoteGameSettings settings)
    {
        foreach (var player in Players())
            _userVoteGameSettingsMap.Add(player.UserId, settings);

        foreach (var individualSettings in settings.RandomIndividualVoteGameSettings)
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

    private IReadOnlyList<string> GetUserChoices(Guid userId)
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
        var variants = GetUserChoices(userId);
        if (!_userVoteGameSettingsMap[userId].MultiplyChoice)
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

    private void GoToNextGameIfNeeded()
    {
        if (_playerConfirmations.Count == Players().Count && _currentVoteStatus == VoteStatus.ShowResult)
            Context.TransitionTo(GetNextGameState());
    }
}