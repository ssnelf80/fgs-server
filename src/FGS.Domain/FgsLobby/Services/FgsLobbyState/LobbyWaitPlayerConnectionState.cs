using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Services.FgsLobbyState;

public class LobbyWaitPlayerConnectionState : AbstractLobbyState
{
    public LobbyWaitPlayerConnectionState(LobbySettings lobbySettings, Dictionary<Guid, Player> playersMap) : base(lobbySettings, playersMap)
    {
        ValidateLobbySettingsOrThrow(lobbySettings);
        InitRandom(lobbySettings.RandomizerSeed);
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.WaitPlayers;

    public override void Handle(ILobbyStateRequest request)
    {
        switch (request)
        {
            case AddPlayerRequest addPlayerRequest:
                AddPlayer(addPlayerRequest.UserId);
                return;
            case RemovePlayerRequest removePlayerRequest:
                RemovePlayer(removePlayerRequest.UserId);
                return;
            default:
                base.Handle(request);
                break;
        }
    }
    
    private void AddPlayer(Guid userId)
    {
        if (PlayersMap.ContainsKey(userId))
            throw new LobbyGameManagerException($"Игрок с ID : {userId} уже существует");

        PlayersMap.Add(userId, new Player(userId, LobbySettings.StartBalance, PlayerRole.Unknown, false));
        
        if (PlayersMap.Count == LobbySettings.PlayersCount)
            Context.TransitionTo(new LobbyReadyToInitializeState(this));
    }
    private void RemovePlayer(Guid userId)
    {
        if (!PlayersMap.Remove(userId))
            throw new LobbyGameManagerException($"Игрок с ID : {userId} не существует");
    }
    
    private void ValidateLobbySettingsOrThrow(LobbySettings s)
    {
        if (s.PlayersCount < 2)
            throw new LobbyGameManagerException($"Минимальное кол-во игроков для создания лобби: 2. Указано: {s.PlayersCount}");
        if (s.TraitorsCount > s.PlayersCount)
            throw new LobbyGameManagerException($"Кол-во предателей меньше кол-ва игроков. Указано {s.TraitorsCount}/{s.PlayersCount} ");
        if (s.StartBalance < 0)
            throw new LobbyGameManagerException($"Стартовый баланс игроков < 0 {s.StartBalance} ");
    }
}