using FGS.Domain.FgsLobby.Entities;

namespace FGS.Domain.Services;

public interface IConnectionTrackerService
{
    Task ConnectAsync(ConnectionTrackerEntity entity, CancellationToken cancellationToken);
    Task DisconnectAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsUserConnectedAsync(Guid userId, CancellationToken cancellationToken);
    Task<ConnectionTrackerEntity?> GetConnectionTrackerOrDefaultAsync(
        Guid userId, CancellationToken cancellationToken);
}
        