using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record ConnectionTrackerEntity(Guid UserId, Guid LobbyId, LobbyUserRole PlayerRole);