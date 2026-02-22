using EventStore.Client;
using FGS.DAL.Extensions;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;

namespace FGS.DAL.EventSourceRepositories;

public class LobbyRepository(
    EventStoreClient eventStoreClient,
    ILobbyEventJsonConvert jsonConvert
    ) : IAggregateRepository<Lobby, LobbyEvent>
{
    public const string StreamPrefix = "Lobby-";
    public static string GetStreamName(Guid id) => $"{StreamPrefix}{id}";
    public async Task<Lobby> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var rawEvents = eventStoreClient.ReadStreamAsync(Direction.Forwards,
            GetStreamName(id), StreamPosition.Start, cancellationToken : cancellationToken);
       
        List<LobbyEvent> lobbyEvents = [];
        var lastRevision = ulong.MaxValue;

        await foreach (var rawEvent in rawEvents)
        {
            lobbyEvents.Add(rawEvent.GetLobbyEvent());
            lastRevision = rawEvent.OriginalEventNumber.ToUInt64();
        }
        
        if (lastRevision == ulong.MaxValue)
           throw new ArgumentOutOfRangeException("No lobby events found");
        
        return new Lobby(id, lastRevision, lobbyEvents);
    }

    public async Task SaveAsync(Lobby aggregate, CancellationToken cancellationToken)
    {
        if (aggregate.Events.Count == 0)
            return;
        
        var streamName = GetStreamName(aggregate.Id);
        await eventStoreClient.AppendToStreamAsync(
            streamName,
            aggregate.Version,
            GetEventDataList(aggregate),
            cancellationToken: cancellationToken);
        aggregate.CommitEvents();
    }

    public IEnumerable<EventData> GetEventDataList(Lobby aggregate)
    {
        foreach (var e in aggregate.Events)
           yield return new EventData(Uuid.NewUuid(), e.GetType().Name, jsonConvert.Serialize(e));
    }
}

