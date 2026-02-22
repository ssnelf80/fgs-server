using FGS.DAL.ViewModelRepository.Entities;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FGS.DAL.ViewModelRepository;

public class FgsViewModelRepository(
    FgsViewModelContext db,
    ILogger<FgsViewModelRepository> logger
    ) : IFgsViewModelRepository
{
    public async Task<IReadOnlyCollection<LobbyEntity>> GetLobbyEntitiesListAsync(LobbyEntitySearchFilter searchFilter, CancellationToken cancellationToken) =>
        await db.Lobbies.Where(searchFilter.GetWhereExpression())
            .OrderByDescending(x => x.CreatedAt)
            .Skip(searchFilter.Offset)
            .Take(searchFilter.Limit)
            .ToListAsync(cancellationToken);

    public async Task<(ulong CommitPosition, ulong PreparePosition)?> GetCurrentLobbyStreamPositionAsync(CancellationToken cancellationToken)
    {
        var tracker = await db.StreamTrackers.FirstOrDefaultAsync(x =>
            x.StreamTypeId == (int)EventSourceStreamTracker.StreamType.Lobby, cancellationToken: cancellationToken);
        if (tracker == null)
            return null;
        return (tracker.CommitPosition, tracker.PreparePosition);
    }

    public async Task SetCurrentLobbyStreamPositionAsync(ulong commitPosition, ulong preparePosition,
        CancellationToken cancellationToken)
    {
        if (!await db.StreamTrackers
                .AnyAsync(x 
                    => x.StreamTypeId == (int)EventSourceStreamTracker.StreamType.Lobby, cancellationToken))
        {
            await db.StreamTrackers
                .AddAsync(new EventSourceStreamTracker((int)EventSourceStreamTracker.StreamType.Lobby, commitPosition, preparePosition), cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            return;
        }

        await db.StreamTrackers
            .Where(x => x.StreamTypeId == (int)EventSourceStreamTracker.StreamType.Lobby)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(p => p.PreparePosition, preparePosition)
                    .SetProperty(p => p.CommitPosition, commitPosition), cancellationToken);
    }

    public async Task<bool> Visit(LobbyCreatedEvent e, CancellationToken ct = default)
    {
        if (db.Lobbies.Any(x => x.Id == e.LobbyId))
        {
            logger.LogWarning($"Lobby with id {e.LobbyId} has already been created");
            return false;
        }
        await db.Lobbies.AddAsync(new LobbyEntity(
            e.LobbyId,
            e.Name,
            e.MasterUserId,
            LobbyStatus.Open,
            e.LobbySettings.PlayersCount,
            [],
            e.CreatedAt,
            DateTimeOffset.UtcNow
        ), ct);
        await db.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<bool> Visit(LobbyStatusChangedEvent e, CancellationToken ct = default)
    {
        var query = db.Lobbies.Where(x => x.Id == e.LobbyId);
        var lobby = await query.FirstOrDefaultAsync(ct);
        if (lobby == null)
        {
            logger.LogWarning($"Lobby with id {e.LobbyId} has not been created");
            return false;
        }
        await query.ExecuteUpdateAsync(x => x.SetProperty(
            p => p.Status, e.Status), ct);
        return true;
    }

    public async Task<bool> Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default)
    {
        var query = db.Lobbies.Where(x => x.Id == e.LobbyId);
        var lobby = await query.FirstOrDefaultAsync(ct);
        if (lobby == null)
        {
            logger.LogWarning($"Lobby with id {e.LobbyId} has not been created");
            return false;
        }
       
        var newUserList = lobby.ConnectedUsers.Concat([e.UserId]).ToList();
        await query.ExecuteUpdateAsync(x => x.SetProperty(
            p => p.ConnectedUsers, newUserList), ct);
        return true;
    }

    public async Task<bool> Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
    {
        var query = db.Lobbies.Where(x => x.Id == e.LobbyId);
        var lobby = await query.FirstOrDefaultAsync(ct);
        if (lobby == null)
        {
            logger.LogWarning($"Lobby with id {e.LobbyId} has not been created");
            return false;
        }
        
        var newUserList = lobby.ConnectedUsers.Where(x => x != e.UserId).ToList();
        await query.ExecuteUpdateAsync(x => x.SetProperty(
            p => p.ConnectedUsers, newUserList), ct);
        return true;
    }
}