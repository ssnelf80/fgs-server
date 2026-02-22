using System.Text.Json;
using EventStore.Client;
using FGS.DAL.EventSourceRepositories;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FGS.DAL.BackgroundServices;

public class EventStoreBackgroundService(
    EventStoreClient eventStoreClient,
    IServiceProvider serviceProvider,
    ILogger<EventStoreBackgroundService> logger
    ) : BackgroundService
{
    private static SubscriptionFilterOptions FilterOptions
        => new(StreamFilter.Prefix(LobbyRepository.StreamPrefix));
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Position currentPosition = Position.Start;
                using (var scope = serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;  
                    var viewModelRepository = services.GetRequiredService<IFgsViewModelRepository>();
                    var offset = await viewModelRepository.GetCurrentLobbyStreamPositionAsync(stoppingToken);
                    
                    if (offset != null)
                        currentPosition = new Position(offset.Value.CommitPosition, offset.Value.PreparePosition);
                }
               
                var subscription = await eventStoreClient.SubscribeToAllAsync(
                    FromAll.After(currentPosition),
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
            }
        }
    }

    private void SubscriptionDropped(StreamSubscription sub, SubscriptionDroppedReason reason, Exception? ex)
    {
        logger.LogError(ex, "Subscription Dropped. Reason: {reason}", reason);
    }

    private async Task EventAppeared(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;  
        var viewModelRepository = services.GetRequiredService<IFgsViewModelRepository>();
        
        LobbyEvent? lobbyEvent = resolvedEvent.Event.EventType switch
        {
            nameof(LobbyCreatedEvent) => JsonSerializer.Deserialize<LobbyCreatedEvent>(resolvedEvent.Event.Data.Span),
            nameof(PlayerConnectedLobbyEvent) => JsonSerializer.Deserialize<PlayerConnectedLobbyEvent>(resolvedEvent.Event.Data.Span),
            nameof(PlayerDisconnectedLobbyEvent) => JsonSerializer.Deserialize<PlayerDisconnectedLobbyEvent>(resolvedEvent.Event.Data.Span),
            nameof(LobbyStatusChangedEvent) => JsonSerializer.Deserialize<LobbyStatusChangedEvent>(resolvedEvent.Event.Data.Span),
            _ => null
        };

        if (lobbyEvent != null)
            await lobbyEvent.Accept(viewModelRepository, cancellationToken);
        else
            logger.LogWarning("recieve unknown lobby event type: {EventEventType}", resolvedEvent.Event.EventType);
        
        await viewModelRepository.SetCurrentLobbyStreamPositionAsync(
            resolvedEvent.Event.Position.CommitPosition, 
            resolvedEvent.Event.Position.PreparePosition,
            cancellationToken);
    }

   
}