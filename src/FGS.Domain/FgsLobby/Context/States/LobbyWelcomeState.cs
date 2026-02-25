using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public sealed class LobbyWelcomeState : LobbyState
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.GameWelcomeInformation;

    protected override void DoBotActions()
    {
        foreach (var bot in BotPlayers())
            _playerConfirmations.Add(bot.UserId);
    }

    private readonly HashSet<Guid> _playerConfirmations = [];

    public LobbyWelcomeState(LobbyState other) : base(other)
    {
        DoBotActions();
        GoToNextGameIfNeeded();
    }

    public override void Handle(ILobbyContextRequest request)
    {
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

    private void GoToNextGameIfNeeded()
    {
        if (_playerConfirmations.Count == Players().Count)
            Context.TransitionTo(GetNextGameState());
    }
    
    protected override PlayerGameState GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        return new PlayerGameState
        {
            Balance = player.Balance,
            PlayerRole = player.Role,
            GameState = GameState,
            InnerGameState = string.Empty,
            GameNumber = CurrentGameNumber,
            Choices = ConfirmationChoice,
            SelectedChoices = ConfirmationChoice,
            CanSendChoice = true,
            GameInfoMessage = LobbySettings.WelcomeMessage,
            RoundInfoMessage = string.Empty
        };
    }
}