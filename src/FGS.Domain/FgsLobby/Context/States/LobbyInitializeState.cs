using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyInitializeState : LobbyState
{
    public bool IsSuccess { get; private set; } = false;
    public LobbyInitializeState(LobbyState other) : base(other)
    {
        InitPlayerRoles();
        InitStartBalance();
        IsSuccess = true;
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.ReadyToInitialize;
    
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
}