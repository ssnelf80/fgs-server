using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States.Base;

public abstract class LobbyGameBase<TGameSettings, TPlayerSettings> : LobbyConfirmationBase
{
    protected int RoundNumber { get; set; } = 0;

    protected enum GameStatus
    {
        InGame,
        ShowResult
    }
    protected GameStatus CurrentGameStatus
    {
        get;
        set
        {
            if (value == GameStatus.ShowResult)
                IsConfirmationMode = true;

            field = value;
        }
    } = GameStatus.InGame;
    
    protected readonly Dictionary<Guid, IReadOnlyList<string>> UserChoicesMap = [];
    protected readonly IGameSettings<TGameSettings, TPlayerSettings> GameSettings;
    protected readonly Dictionary<Guid, TPlayerSettings> UserGameSettingsMap = [];
    protected readonly HashSet<Guid> UsersWithIndividualGameSettings = [];

    protected LobbyGameBase(LobbyState other, IGameSettings<TGameSettings, TPlayerSettings> gameSettings) : base(other, false)
    {
        GameSettings = gameSettings;
        InitUserGameSettings();
        DoBotActions();
        GoToNextGameIfNeeded();
    }

    protected abstract IReadOnlyList<string> GetUserRandomChoices(Guid userId);

    protected sealed override void DoBotActions()
    {
        if (IsConfirmationMode)
        {
            base.DoBotActions();
        }
        else
        {
            foreach (var player in BotPlayers())
                UserChoicesMap[player.UserId] = GetUserRandomChoices(player.UserId);
            
            SetResultIfNeeded();
        }

        GoToNextGameIfNeeded();
    }
    
    protected void SetResultIfNeeded()
    {
        if (!CanSetResult()) 
            return;

        InnerSetResult();
        DoBotActions();
    }
    
    protected abstract GameStatus SetResult();
    
    private void InnerSetResult()
    {
        if (!CanSetResult())
            throw new InvalidOperationLobbyStateException(
                $"can't set result: {CurrentGameStatus} and {UserChoicesMap.Count}/{Players().Count}");
        
        CurrentGameStatus = SetResult();

        if (CurrentGameStatus == GameStatus.InGame)
        {
            RoundNumber++;
            UserGameSettingsMap.Clear();
            UserChoicesMap.Clear();
            InitUserGameSettings();
        }
    }

    private bool CanSetResult() => CurrentGameStatus == GameStatus.InGame && UserChoicesMap.Count == Players().Count;
    
    private void InitUserGameSettings()
    {
        foreach (var player in Players())
            UserGameSettingsMap.Add(player.UserId, GameSettings.DefaultPlayerSettings);

        RoundSetLocalSettings();
    }

    protected abstract void RoundSetLocalSettings();

    protected void SetLocalPlayerSettings(TPlayerSettings localPlayerSettings)
    {
        if (UsersWithIndividualGameSettings.Count == Players().Count) // todo log?
            return;

        var rndPlayer = GetRandomPlayerWithoutIndividualVoteGameSettings();
        UsersWithIndividualGameSettings.Add(rndPlayer.UserId);
        UserGameSettingsMap[rndPlayer.UserId] = localPlayerSettings;
    }
    
    protected Player GetRandomPlayerWithoutIndividualVoteGameSettings()
        => GetRandomPlayer(Players()
            .Where(x => !UsersWithIndividualGameSettings.Contains(x.UserId))
            .OrderBy(x => x.UserId)
            .ToList());

    public sealed override void Handle(ILobbyContextRequest request)
    {
        if (CurrentGameStatus == GameStatus.ShowResult)
        {
            base.Handle(request);
            return;
        }
        
        switch (request)
        {
            case SetUserChoicesRequest userChoicesRequest:
                var validationResult = InnerValidate(userChoicesRequest);
                if (!validationResult.IsSuccess)
                    throw validationResult.Exception;
                
                if (userChoicesRequest.Choices.Length == 0)
                {
                    UserChoicesMap.Remove(userChoicesRequest.UserId);
                }
                else
                {
                    UserChoicesMap[userChoicesRequest.UserId] = userChoicesRequest.Choices;
                    SetResultIfNeeded();
                }
                break;
            case SetRandomUserChoicesRequest randomChoiceRequest when !IsPlayerExists(randomChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {randomChoiceRequest.UserId} is not exist");
            case SetRandomUserChoicesRequest rndUserChoicesRequest:
                UserChoicesMap[rndUserChoicesRequest.UserId] = GetUserRandomChoices(rndUserChoicesRequest.UserId);
                SetResultIfNeeded();
                break;
            default:
                base.Handle(request);
                break;
        }
    }

    private ValidationResult InnerValidate(SetUserChoicesRequest request)
    {
        if (!IsPlayerExists(request.UserId))
            return ValidationResult.Failure(new LobbyStateException($"player with id = {request.UserId} is not exist"));
        
        return Validate(request);
    }
    
    protected abstract ValidationResult Validate(SetUserChoicesRequest request);
    
    protected sealed override void GoToNextGameIfNeeded()
    {
        if (CurrentGameStatus == GameStatus.ShowResult && IsConfirmed())
            Context.TransitionTo(GetNextGameState());
    }
    
    protected IReadOnlyCollection<string> GetUserSelectedChoices(Guid userId)
    {
        if (UserChoicesMap.TryGetValue(userId, out var choices))
            return choices;
        return [];
    }
}