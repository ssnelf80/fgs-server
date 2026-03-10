using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public sealed class LobbyWelcomeState : LobbyConfirmationBase
{
    public LobbyWelcomeState(LobbyState other) : base(other, true)
    {
        DoBotActions();
        GoToNextGameIfNeeded();
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.GameWelcomeInformation;

    protected override void GoToNextGameIfNeeded()
    {
        if (IsConfirmed())
            Context.TransitionTo(GetNextGameState());
    }
    
    public override PlayerGameState GetPlayerGameState(Guid userId)
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
            SelectedChoices = [],
            CanSendChoice = true,
            GameInfoMessage = LobbySettings.WelcomeMessage,
            RoundInfoMessage = string.Empty
        };
    }
}