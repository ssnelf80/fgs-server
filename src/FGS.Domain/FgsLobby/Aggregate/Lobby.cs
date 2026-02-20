using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.FgsLobby.Services;

namespace FGS.Domain.FgsLobby.Aggregate;

public sealed partial class Lobby : AggregateRoot<LobbyEvent>
{
    private LobbyGameManager? _lobbyGameManager;
    private LobbyGameManager LobbyGameManager => _lobbyGameManager ?? throw new DomainException("LobbyGameManager is not initialized");
    private readonly InnerLobbyManagerVisitor _innerLobbyManagerVisitor;
    public LobbyStatus Status { get; private set; }
    
    private Lobby(Guid id, long version, IReadOnlyCollection<LobbyEvent> commitedEvents) : base(id, version)
    {
        _innerLobbyManagerVisitor = new InnerLobbyManagerVisitor(this);
        foreach (var e in commitedEvents)
            ApplyChanges(e);
    }

    public void ConnectUser(Guid userId)
    {
        EmitEvent(new PlayerConnectedLobbyEvent(Id, userId));
        if (LobbyGameManager.LobbyGameState != LobbyGameState.WaitPlayers)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.InProgress));
    }

    protected override void ApplyChanges(LobbyEvent e) => e.Accept(_innerLobbyManagerVisitor);
}



