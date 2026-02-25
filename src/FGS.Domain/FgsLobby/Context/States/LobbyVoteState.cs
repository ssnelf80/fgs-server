using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public sealed class LobbyVoteState : LobbyState
{
    public enum VoteStatus
    {
        Vote,
        ShowResult
    }

    private VoteStatus CurrentVoteStatus = VoteStatus.Vote;
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

    protected override void DoBotActions()
    {
        foreach (var player in BotPlayers())
        {
            if (CurrentVoteStatus == VoteStatus.Vote)
            {
                _userChoicesMap.Add(player.UserId, GetUserRandomChoices(player.UserId));
                throw new NotImplementedException(); // todo повторный вызов подтверждения, если нужно
            }
            else if (CurrentVoteStatus == VoteStatus.ShowResult)
            {
                _playerConfirmations.Add(player.UserId);
            }
        }
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

    private IReadOnlyList<string> GetUserVariants(Guid userId)
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
        var variants = GetUserVariants(userId);
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
        if (_playerConfirmations.Count == Players().Count)
            Context.TransitionTo(GetNextGameState());
    }
}