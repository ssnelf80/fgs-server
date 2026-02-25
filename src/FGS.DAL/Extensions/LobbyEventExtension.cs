using System.Text.Json;
using EventStore.Client;
using FGS.Domain.FgsLobby.Events;

namespace FGS.DAL.Extensions;

public static class LobbyEventExtension
{
    public static LobbyEvent? GetLobbyEventOrDefault(this ResolvedEvent resolvedEvent)
    {
        return resolvedEvent.Event.EventType switch
        {
            nameof(LobbyCreatedEvent) => JsonSerializer.Deserialize<LobbyCreatedEvent>(
                resolvedEvent.Event.Data.Span),
            nameof(PlayerConnectedLobbyEvent) => JsonSerializer.Deserialize<PlayerConnectedLobbyEvent>(resolvedEvent
                .Event.Data.Span),
            nameof(PlayerDisconnectedLobbyEvent) => JsonSerializer.Deserialize<PlayerDisconnectedLobbyEvent>(
                resolvedEvent.Event.Data.Span),
            nameof(LobbyStatusChangedEvent) => JsonSerializer.Deserialize<LobbyStatusChangedEvent>(resolvedEvent
                .Event.Data.Span),
            nameof(LobbySetUserChoiceEvent) => JsonSerializer.Deserialize<LobbySetUserChoiceEvent>(resolvedEvent
                .Event.Data.Span),
            nameof(LobbySetUserRandomChoiceEvent) => JsonSerializer.Deserialize<LobbySetUserRandomChoiceEvent>(resolvedEvent
                .Event.Data.Span),
            nameof(SetBotToPlayerLobbyEvent) => JsonSerializer.Deserialize<SetBotToPlayerLobbyEvent>(resolvedEvent
                .Event.Data.Span),
            nameof(RemoveBotFromPlayerLobbyEvent) => JsonSerializer.Deserialize<RemoveBotFromPlayerLobbyEvent>(resolvedEvent
                .Event.Data.Span),
            _ => null
        };
    }

    public static LobbyEvent GetLobbyEvent(this ResolvedEvent resolvedEvent) => resolvedEvent.GetLobbyEventOrDefault() ?? throw new ArgumentOutOfRangeException(resolvedEvent.Event.EventType);
}