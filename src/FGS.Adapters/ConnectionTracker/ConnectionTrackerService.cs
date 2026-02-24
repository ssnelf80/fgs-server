using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.Services;

namespace FGS.Adapters.ConnectionTracker;

public class ConnectionTrackerService : IConnectionTrackerService
{
    public Task ConnectAsync(Guid userId, Guid lobbyId, PlayerRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}