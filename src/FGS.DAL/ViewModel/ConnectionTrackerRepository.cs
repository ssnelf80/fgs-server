using FGS.DAL.Context;
using FGS.Domain.FgsLobby.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FGS.DAL.ViewModel;

public class ConnectionTrackerRepository(
    FgsViewModelContext db, 
    ILogger<ConnectionTrackerRepository> logger
    ) : ViewModelRepositoryBase(db, logger)
{
    public Task<bool> IsUserConnectedAsync(Guid userId, CancellationToken cancellationToken) => Context.UserConnections.AnyAsync(x => x.UserId == userId, cancellationToken);

    public Task<ConnectionTrackerEntity?> GetConnectionTrackerOrDefaultAsync(Guid userId, CancellationToken cancellationToken) 
        => Context.UserConnections.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public Task ConnectUserAsync(ConnectionTrackerEntity entity, CancellationToken cancellationToken) =>
        TransactionWrapperAsync(async () =>
        {
            if (await IsUserConnectedAsync(entity.UserId, cancellationToken))
            {
                throw new InvalidOperationException($"User {entity.UserId} has already been connected");
            }

            await using var addContext = Context.CreateDbContext();
            await addContext.UserConnections.AddAsync(entity, cancellationToken);
            await addContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

    public Task DisconnectUserAsync(Guid userId, CancellationToken cancellationToken) =>
        TransactionWrapperAsync(async () =>
        {
            if (!await IsUserConnectedAsync(userId, cancellationToken))
            {
                Logger.LogWarning($"User {userId} has not been connected");
                return;
            }

            await Context.UserConnections.Where(x => x.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

        }, cancellationToken);
}