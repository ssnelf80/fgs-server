using FGS.DAL.ViewModel;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.Services;

namespace FGS.Adapters.ConnectionTracker;

public class ConnectionTrackerService(
    ConnectionTrackerRepository connectionTrackerRepository
    ) : IConnectionTrackerService
{
    public Task ConnectAsync(ConnectionTrackerEntity entity, CancellationToken cancellationToken) 
        => connectionTrackerRepository.ConnectUserAsync(entity, cancellationToken);

    public Task DisconnectAsync(Guid userId, CancellationToken cancellationToken) 
        => connectionTrackerRepository.DisconnectUserAsync(userId, cancellationToken);

    public Task<bool> IsUserConnectedAsync(Guid userId, CancellationToken cancellationToken) 
        => connectionTrackerRepository.IsUserConnectedAsync(userId, cancellationToken);

    public Task<ConnectionTrackerEntity?> GetConnectionTrackerOrDefaultAsync(Guid userId, CancellationToken cancellationToken) 
        => connectionTrackerRepository.GetConnectionTrackerOrDefaultAsync(userId, cancellationToken);
}