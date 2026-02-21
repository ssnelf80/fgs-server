using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyReadyToInitializeState(LobbyState other) : LobbyState(other)
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.ReadyToInitialize;
    public override void Handle(ILobbyContextRequest request)
    {
        if (request is InitializeGameRequest)
        {
            InitPlayerRoles();
            InitStartBalance();
            return;
        }
        base.Handle(request);
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
}