using FGS.DAL.Context;
using FGS.DAL.ViewModel.Entities;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FGS.DAL.ViewModel;

public class FgsViewModelRepository(
    FgsViewModelContext db,
    ILogger<FgsViewModelRepository> logger
    ) : ViewModelRepositoryBase(db, logger), IFgsViewModelRepository
{
    public async Task<IReadOnlyCollection<LobbyEntity>> GetLobbyEntitiesListAsync(LobbyEntitySearchFilter searchFilter, CancellationToken cancellationToken) =>
        await Context.Lobbies.Where(searchFilter.GetWhereExpression())
            .OrderByDescending(x => x.CreatedAt)
            .Skip(searchFilter.Offset)
            .Take(searchFilter.Limit)
            .ToListAsync(cancellationToken);

    public async Task<(ulong CommitPosition, ulong PreparePosition)?> GetCurrentLobbyStreamPositionAsync(CancellationToken cancellationToken)
    {
        var tracker = await Context.StreamTrackers.FirstOrDefaultAsync(x =>
            x.StreamTypeId == (int)EventSourceStreamTracker.StreamType.Lobby, cancellationToken: cancellationToken);
        if (tracker == null)
            return null;
        return (tracker.CommitPosition, tracker.PreparePosition);
    }

    public Task SetCurrentLobbyStreamPositionAsync(ulong commitPosition, ulong preparePosition,
        CancellationToken cancellationToken) =>
        TransactionWrapperAsync(async () =>
        {
            if (!await Context.StreamTrackers
                    .AnyAsync(x 
                        => x.StreamTypeId == (int)EventSourceStreamTracker.StreamType.Lobby, cancellationToken))
            {
                await Context.StreamTrackers
                    .AddAsync(new EventSourceStreamTracker((int)EventSourceStreamTracker.StreamType.Lobby, commitPosition, preparePosition), cancellationToken);
                await Context.SaveChangesAsync(cancellationToken);
                return;
            }

            await Context.StreamTrackers
                .Where(x => x.StreamTypeId == (int)EventSourceStreamTracker.StreamType.Lobby)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.PreparePosition, preparePosition)
                        .SetProperty(p => p.CommitPosition, commitPosition), cancellationToken);
        }, cancellationToken);

    public async Task<bool> Visit(LobbyCreatedEvent e, CancellationToken ct = default)
    {
        var result = true;
        await TransactionWrapperAsync(async () =>
        {
            if (Context.Lobbies.Any(x => x.Id == e.LobbyId))
            {
                Logger.LogWarning($"Lobby with id {e.LobbyId} has already been created");
                result = false;
                return;
            }

            await Context.Lobbies.AddAsync(new LobbyEntity(
                e.LobbyId,
                e.Name,
                e.MasterUserId,
                LobbyStatus.Open,
                e.LobbySettings.PlayersCount,
                [],
                e.CreatedAt,
                DateTimeOffset.UtcNow
            ), ct);
            await Context.SaveChangesAsync(ct);
        }, ct);
        return result;
    }

    public async Task<bool> Visit(LobbyStatusChangedEvent e, CancellationToken ct = default)
    {
        var result = true;
        await TransactionWrapperAsync(async () =>
        {
            var query = Context.Lobbies.Where(x => x.Id == e.LobbyId);
            var lobby = await query.FirstOrDefaultAsync(ct);
            if (lobby == null)
            {
                Logger.LogWarning($"Lobby with id {e.LobbyId} has not been created");
                result = false;
                return;
            }

            await query.ExecuteUpdateAsync(x => x.SetProperty(
                p => p.Status, e.Status), ct);
        }, ct);
        
        return result;
    }

    public async Task<bool> Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default)
    {
        var result = true;
        await TransactionWrapperAsync(async () =>
        {
            var query = Context.Lobbies.Where(x => x.Id == e.LobbyId);
            var lobby = await query.FirstOrDefaultAsync(ct);
            if (lobby == null)
            {
                Logger.LogWarning($"Lobby with id {e.LobbyId} has not been created");
                result = false;
                return;
            }
       
            var newUserList = lobby.ConnectedUsers.Concat([e.UserId]).ToList();
            await query.ExecuteUpdateAsync(x => x.SetProperty(
                p => p.ConnectedUsers, newUserList), ct);
        }, ct);
       
        return result;
    }

    public async Task<bool> Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
    {
        var result = true;
        await TransactionWrapperAsync(async () =>
        {
            var query = Context.Lobbies.Where(x => x.Id == e.LobbyId);
            var lobby = await query.FirstOrDefaultAsync(ct);
            if (lobby == null)
            {
                Logger.LogWarning($"Lobby with id {e.LobbyId} has not been created");
                result = false;
                return;
            }
        
            var newUserList = lobby.ConnectedUsers.Where(x => x != e.UserId).ToList();
            await query.ExecuteUpdateAsync(x => x.SetProperty(
                p => p.ConnectedUsers, newUserList), ct);
        }, ct);
       
        return result;
    }

    
}