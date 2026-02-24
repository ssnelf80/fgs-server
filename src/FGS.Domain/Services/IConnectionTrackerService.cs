using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.Services;

public interface IConnectionTrackerService
{
    public Task ConnectAsync(Guid userId, Guid lobbyId, PlayerRole role, CancellationToken cancellationToken);
    public Task DisconnectAsync(Guid userId, CancellationToken cancellationToken);
}