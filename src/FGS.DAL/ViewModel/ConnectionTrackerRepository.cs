using FGS.DAL.Context;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.DAL.ViewModel;

public class ConnectionTrackerRepository(FgsViewModelContext db)
{
    public Task ConnectUserAsync(Guid userId, Guid lobbyId, PlayerRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}