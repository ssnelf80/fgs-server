using System.Runtime.CompilerServices;
using System.Text.Json;
using EventStore.Client;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;

namespace FGS.DAL;

public class LobbyRepository(
    EventStoreClient eventStoreClient,
    ILobbyEventJsonConvert jsonConvert
    ) : IAggregateRepository<Lobby, LobbyEvent>
{
    public static string GetStreamName(Guid id) => $"Lobby-{id}";
    public Task<Lobby> GetAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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

