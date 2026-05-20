using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyInitializeState(LobbyState other) : LobbyState(other, false)
{
    protected override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.ReadyToInitialize;

    public override void Handle(ILobbyContextRequest request)
    {
        if (request is not InitializeGameRequest)
        {
            base.Handle(request);
            return;
        }
        
        InitPlayerRoles();
        InitStartBalance();
        Context.TransitionTo(new LobbyWelcomeState(this));
    }

    private void InitPlayerRoles()
    {
        foreach (var player in Players())
            UpdatePlayer(player with { Role = PlayerRole.Innocent });

        for (var i = 0; i < LobbySettings.TraitorsCount; i++)
        {
            var player = GetRandomPlayer(InnocentPlayers());
            UpdatePlayer(player with { Role = PlayerRole.Traitor });
        }
    }
    
    private void InitStartBalance()
    {
        foreach (var player in Players())
            UpdatePlayer(player with { Balance = LobbySettings.StartBalance });
    }
    
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        return new PlayerStateWrapper
        {
            Balance = player.Balance,
            PlayerRole = player.Role,
            LobbyGameType = GameState,
            GameState = EmptyState.Instance,
            GameNumber = CurrentGameNumber,
            Message = "Инициализация игры, ожидайте"
        };
    }
}