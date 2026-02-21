using EventStore.Client;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Events;

namespace FGS.DAL;

public class LobbyRepository(EventStoreClient eventStoreClient) : IAggregateRepository<Lobby, LobbyEvent>
{
    public Task<Lobby> GetAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Lobby aggregate, CancellationToken cancellationToken)
    {
         //aggregate.Events
         var streamName = aggregate.Id.ToString();
         eventStoreClient.AppendToStreamAsync(streamName, aggregate.Version,)
        
    }
}

