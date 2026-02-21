using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Services.FgsLobbyState;

public class LobbyReadyToInitializeState(AbstractLobbyState other) : AbstractLobbyState(other)
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.ReadyToInitialize;
    public override void Handle(ILobbyStateRequest request)
    {
        if (request is InitializeGameRequest)
        {
            Init();
            return;
        }
        base.Handle(request);
    }

    private void Init()
    {
        InitPlayerRoles();
        InitStartBalance();
    }
    
    private void InitPlayerRoles()
    {
        // todo закрыть нафиг мапу
        foreach (var uid in PlayersMap.Keys)
            PlayersMap[uid] = PlayersMap[uid] with { Role = PlayerRole.Innocent };

        for (var i = 0; i < LobbySettings.TraitorsCount; i++)
        {
            var rndPlayerId = GetRandomPlayerUserId(InnocentPlayers());
            PlayersMap[rndPlayerId] = PlayersMap[rndPlayerId] with { Role = PlayerRole.Traitor };
        }
    }
    
    private void InitStartBalance()
    {
        foreach (var uid in PlayersMap.Keys)
            PlayersMap[uid] = PlayersMap[uid] with { Balance = LobbySettings.StartBalance };
    }
}