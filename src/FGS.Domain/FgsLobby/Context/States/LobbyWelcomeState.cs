using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;
using FGS.Domain.FgsLobby.Context.States.Base;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public sealed class LobbyWelcomeState : LobbyConfirmationBase
{
    public LobbyWelcomeState(LobbyState other) : base(other, true)
    {
        DoBotActions();
        GoToNextGameIfNeeded();
    }

    public override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.GameWelcomeInformation;

    protected override void GoToNextGameIfNeeded()
    {
        if (IsConfirmed())
            Context.TransitionTo(GetNextGameState());
    }
    
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        return new PlayerStateWrapper
        {
            Balance = player.Balance,
            PlayerRole = player.Role,
            LobbyGameType = GameState,
            GameNumber = CurrentGameNumber,
            Message = LobbySettings.WelcomeMessage,
            GameState = new ConfirmationState(IsPlayerConfirm(userId))
        };
    }
}