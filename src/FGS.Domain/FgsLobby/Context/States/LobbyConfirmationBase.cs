using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public abstract class LobbyConfirmationBase : LobbyState
{
    protected bool IsConfirmationMode { get; set; }
    private readonly HashSet<Guid> _playerConfirmations = [];
    
    protected LobbyConfirmationBase(LobbyState other, bool isConfirmationMode) : base(other)
    {
        IsConfirmationMode = isConfirmationMode;
    }
    
    protected bool IsPlayerConfirm(Guid userId) => _playerConfirmations.Contains(userId);
    
    protected bool IsConfirmed() => _playerConfirmations.Count == Players().Count;

    protected override void DoBotActions()
    {
        if (!IsConfirmationMode) 
            return;
        
        foreach (var bot in BotPlayers())
            _playerConfirmations.Add(bot.UserId);
    }
    
    public override void Handle(ILobbyContextRequest request)
    {
        if (!IsConfirmationMode)
        {
            base.Handle(request);
            return;
        }

        switch (request)
        {
            case SetUserChoicesRequest userChoiceRequest when !IsPlayerExists(userChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {userChoiceRequest.UserId} is not exist");
            case SetUserChoicesRequest userChoiceRequest:
            {
                if (userChoiceRequest.Choices.Length == 0)
                    _playerConfirmations.Remove(userChoiceRequest.UserId);
                else
                    _playerConfirmations.Add(userChoiceRequest.UserId);

                GoToNextGameIfNeeded();
                return;
            }
            case SetRandomUserChoicesRequest randomChoiceRequest when !IsPlayerExists(randomChoiceRequest.UserId):
                throw new LobbyStateException($"player with id = {randomChoiceRequest.UserId} is not exist");
            case SetRandomUserChoicesRequest randomChoiceRequest:
                _playerConfirmations.Add(randomChoiceRequest.UserId);

                GoToNextGameIfNeeded();
                return;
            default:
                base.Handle(request);
                break;
        }
    }
    
    protected abstract void GoToNextGameIfNeeded();
}