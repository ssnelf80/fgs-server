using FGS.DAL.ViewModel;
using FGS.Domain.FgsLobby.Entities;

namespace FGS.App;

public class UserConnectionService(
    ConnectionTrackerRepository connectionTrackerRepository
    )
{
    public Task<ConnectionTrackerEntity?> GetConnectionTrackerOrDefault(Guid userId, CancellationToken cancellationToken) 
        => connectionTrackerRepository.GetConnectionTrackerOrDefaultAsync(userId, cancellationToken);
}