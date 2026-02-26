using EventStore.Client;
using FGS.Adapters.Hubs;
using FGS.DAL.EventSourceRepositories;
using FGS.DAL.Extensions;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FGS.Adapters.BackgroundServices;

public class EventStoreBackgroundService(
    EventStoreClient eventStoreClient,
    IServiceProvider serviceProvider,
    ILogger<EventStoreBackgroundService> logger,
    IHubContext<LobbyHub> lobbyHub
) : BackgroundService
{
    private static Position CurrentPosition = Position.Start;
    private static CancellationToken StoppingToken = CancellationToken.None;

    private static SubscriptionFilterOptions FilterOptions
        => new(StreamFilter.Prefix(LobbyRepository.StreamPrefix));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        
        StoppingToken = stoppingToken;

        try
        {
            if (CurrentPosition == Position.Start)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var viewModelRepository = services.GetRequiredService<IFgsViewModelRepository>();
                    var offset = await viewModelRepository.GetCurrentLobbyStreamPositionAsync(stoppingToken);

                    if (offset != null)
                        CurrentPosition = new Position(offset.Value.CommitPosition, offset.Value.PreparePosition);
                }
            }

            var subscription = await eventStoreClient.SubscribeToAllAsync(
                FromAll.After(CurrentPosition),
                EventAppeared,
                false,
                subscriptionDropped: SubscriptionDropped,
                FilterOptions,
                null,
                stoppingToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"background event store listener error: {ex.Message}");
            _ = ExecuteAsync(StoppingToken);
        }
    }

    private void SubscriptionDropped(StreamSubscription sub, SubscriptionDroppedReason reason, Exception? ex)
    {
        logger.LogError(ex, "Subscription Dropped. Reason: {reason}", reason);
    }

    private async Task EventAppeared(StreamSubscription subscription, ResolvedEvent resolvedEvent,
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var viewModelRepository = scope.ServiceProvider.GetRequiredService<IFgsViewModelRepository>();

        var lobbyEvent = resolvedEvent.GetLobbyEventOrDefault();

        if (lobbyEvent != null)
            await lobbyEvent.Accept(viewModelRepository, cancellationToken);
        else
            logger.LogWarning("recieve unknown lobby event type: {EventEventType}", resolvedEvent.Event.EventType);

        await viewModelRepository.SetCurrentLobbyStreamPositionAsync(
            resolvedEvent.Event.Position.CommitPosition,
            resolvedEvent.Event.Position.PreparePosition,
            cancellationToken);

        CurrentPosition = resolvedEvent.Event.Position;
        
        if (lobbyEvent != null)
            await lobbyHub.Clients.All.SendAsync("lobbyChanged", 
                new LobbyStateChangedHubEvent(lobbyEvent.LobbyId, lobbyEvent.GetType().Name), 
                cancellationToken);
    }
}