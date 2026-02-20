using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Services;

public partial class LobbyGameManager
{
    public bool ReadyToStartInitialize() =>
        LobbyGameState == LobbyGameState.WaitPlayers && _playersMap.Count == _lobbySettings.PlayersCount;
    
    public void InitializeStartState()
    {
        if (!ReadyToStartInitialize())
            throw new LobbyGameManagerException($"Нельзя инициализировать игру. LobbyGameState = {LobbyGameState}, PlayersCount = {_playersMap.Count} {_lobbySettings.PlayersCount}");

        LobbyGameState = LobbyGameState.Initialize;
        
        InitPlayerRoles();
        InitStartBalance();
    }
    private void InitStartBalance()
    {
        foreach (var uid in _playersMap.Keys)
            _playersMap[uid] = _playersMap[uid] with { Balance = _lobbySettings.StartBalance };
    }
    private void InitPlayerRoles()
    {
        foreach (var uid in _playersMap.Keys)
            _playersMap[uid] = _playersMap[uid] with { Role = PlayerRole.Innocent };

        for (var i = 0; i < _lobbySettings.TraitorsCount; i++)
        {
            var rndPlayerId = GetRandomPlayerUserId(InnocentPlayers());
            _playersMap[rndPlayerId] = _playersMap[rndPlayerId] with { Role = PlayerRole.Traitor };
        }
    }
}