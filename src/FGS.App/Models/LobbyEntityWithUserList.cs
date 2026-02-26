using FGS.Domain.FgsLobby.Entities;

namespace FGS.App.Models;

public record LobbyEntityWithUserList(
    IReadOnlyCollection<LobbyEntity> Lobbies,
    IReadOnlyDictionary<Guid, string> UserNames);
    
