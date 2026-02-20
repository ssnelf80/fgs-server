using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.FgsLobby.Exceptions;
using FGS.Domain.FgsLobby.Services;
using LobbyGameManager = FGS.Domain.FgsLobby.Services.GameManager.LobbyGameManager;

namespace FGS.Domain.FgsLobby.Aggregate;

public sealed partial class Lobby : AggregateRoot<LobbyEvent>
{
    private LobbyGameManager? _lobbyGameManager;
    private LobbyGameManager LobbyGameManager => _lobbyGameManager ?? throw new LobbyException("LobbyGameManager is not initialized");
    private readonly InnerLobbyManagerVisitor _innerLobbyManagerVisitor;
    public LobbyStatus Status { get; private set; }
    
    private Lobby(Guid id, long version, IReadOnlyCollection<LobbyEvent> commitedEvents) : base(id, version)
    {
        _innerLobbyManagerVisitor = new InnerLobbyManagerVisitor(this);
        foreach (var e in commitedEvents)
            ApplyChanges(e);
    }

    public static Lobby Create(Guid masterUserId, string name, LobbySettings lobbySettings)
    {
        var lobby = new Lobby(Guid.NewGuid(), 0, []);
        lobby.EmitEvent(new LobbyCreatedEvent(lobby.Id, name, masterUserId, lobbySettings, DateTimeOffset.UtcNow));
        return lobby;
    }
    
    public void DisconnectUser(Guid userId) => EmitEvent(new PlayerDisconnectedLobbyEvent(Id, userId));

    public void ConnectUser(Guid userId)
    {
        EmitEvent(new PlayerConnectedLobbyEvent(Id, userId));
        if (LobbyGameManager.LobbyGameState != LobbyGameState.WaitPlayers)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.InProgress));
    }

    protected override void ApplyChanges(LobbyEvent e) => e.Accept(_innerLobbyManagerVisitor);
}



