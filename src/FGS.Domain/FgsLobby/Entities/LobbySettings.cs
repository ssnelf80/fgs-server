namespace FGS.Domain.FgsLobby.Entities;

public record LobbySettings
(
    uint PlayersCount,
    Guid MasterUserId
    // todo настройки игр
);