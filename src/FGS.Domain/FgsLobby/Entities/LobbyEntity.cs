using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record LobbyEntity(
    Guid Id,
    string Name,
    Guid MasterUserId,
    LobbyStatus Status,
    uint PlayersCount,
    IReadOnlyCollection<Guid> ConnectedUsers,
    DateTimeOffset CreatedAt
);