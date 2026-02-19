using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Events;

namespace FGS.Domain.FgsLobby;

public sealed class Lobby : AggregateRoot<LobbyEvent>
{
    private Lobby(Guid id, long version, IReadOnlyCollection<LobbyEvent> commitedEvents) : base(id, version)
    {
        foreach (var e in commitedEvents)
            ApplyChanges(e);
    }

    protected override void ApplyChanges(LobbyEvent e)
    {
        throw new NotImplementedException();
    }
}

