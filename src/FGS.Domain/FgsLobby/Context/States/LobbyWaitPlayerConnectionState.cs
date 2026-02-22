using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyWaitPlayerConnectionState : LobbyState
{
    public LobbyWaitPlayerConnectionState(LobbySettings lobbySettings, Dictionary<Guid, Player> playersMap) : base(lobbySettings, playersMap, true)
    {
        ValidateLobbySettingsOrThrow(lobbySettings);
        InitRandom(lobbySettings.RandomizerSeed);
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.WaitPlayers;

    public override void Handle(ILobbyContextRequest request)
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
        if (UnsafePlayerMap.ContainsKey(userId))
            throw new LobbyStateException($"Игрок с ID : {userId} уже существует");

        UnsafePlayerMap.Add(userId, new Player(userId, LobbySettings.StartBalance, PlayerRole.Unknown, false));

        if (UnsafePlayerMap.Count == LobbySettings.PlayersCount)
        {
            Context.TransitionTo(new LobbyInitializeState(this));
        }
           
    }
    private void RemovePlayer(Guid userId)
    {
        if (!UnsafePlayerMap.Remove(userId))
            throw new LobbyStateException($"Игрок с ID : {userId} не существует");
    }
    
    private void ValidateLobbySettingsOrThrow(LobbySettings s)
    {
        if (s.PlayersCount < 2)
            throw new LobbyStateException($"Минимальное кол-во игроков для создания лобби: 2. Указано: {s.PlayersCount}");
        if (s.TraitorsCount > s.PlayersCount)
            throw new LobbyStateException($"Кол-во предателей меньше кол-ва игроков. Указано {s.TraitorsCount}/{s.PlayersCount} ");
        if (s.StartBalance < 0)
            throw new LobbyStateException($"Стартовый баланс игроков < 0 {s.StartBalance} ");
    }
}