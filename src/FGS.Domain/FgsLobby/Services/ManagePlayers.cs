using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Services;

public partial class LobbyGameManager
{
    public void AddPlayer(Guid userId)
    {
        if (LobbyGameState != LobbyGameState.WaitPlayers)
            throw new LobbyGameManagerException($"Нельзя добавить игроков. LobbyGameState = {LobbyGameState}");
        
        if (_playersMap.ContainsKey(userId))
            throw new LobbyGameManagerException($"Игрок с ID : {userId} уже существует");

        _playersMap.Add(userId, new Player(userId, _lobbySettings.StartBalance, PlayerRole.Unknown, false));
    }
    public void RemovePlayer(Guid userId)
    {
        if (LobbyGameState != LobbyGameState.WaitPlayers)
            throw new LobbyGameManagerException($"Нельзя исключить игроков. LobbyGameState = {LobbyGameState}");
        
        if (!_playersMap.Remove(userId))
            throw new LobbyGameManagerException($"Игрок с ID : {userId} не существует");
    }
}