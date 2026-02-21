using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.FgsLobby.Exceptions;
using FGS.Domain.FgsLobby.Services.FgsLobbyState;


namespace FGS.Domain.FgsLobby.Aggregate;

public sealed class Lobby : AggregateRoot<LobbyEvent>
{
    private LobbyStateContext _context;
    private  LobbyStateContext Context => _context ?? throw new LobbyException("LobbyContext is not initialized");
    private readonly InnerLobbyManagerVisitor _innerLobbyManagerVisitor;
    public LobbyStatus Status { get; private set; }
    
    private Lobby(Guid id, ulong version, IReadOnlyCollection<LobbyEvent> commitedEvents) : base(id, version)
    {
        _innerLobbyManagerVisitor = new InnerLobbyManagerVisitor(this);
        foreach (var e in commitedEvents)
            ApplyChanges(e);
    }

    public static Lobby Create(Guid masterUserId, string name, LobbySettings lobbySettings)
    {
        var lobby = new Lobby(Guid.NewGuid(), NoVersion, []);
        lobby.EmitEvent(new LobbyCreatedEvent(lobby.Id, name, masterUserId, lobbySettings, DateTimeOffset.UtcNow));
        return lobby;
    }
    
    public void DisconnectUser(Guid userId) => EmitEvent(new PlayerDisconnectedLobbyEvent(Id, userId));

    public void ConnectUser(Guid userId)
    {
        EmitEvent(new PlayerConnectedLobbyEvent(Id, userId));
        if (Context.Status == LobbyGameStateEnum.ReadyToInitialize)
        {
            Context.SendRequest(new InitializeGameRequest());
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.InProgress));
        }
    }

    public void InitContext(LobbySettings settings)
    {
        if (_context is not null)
            throw new LobbyException("LobbyContext already initialized");
        _context = LobbyStateContext.Create(settings);
    }

    protected override void ApplyChanges(LobbyEvent e) => e.Accept(_innerLobbyManagerVisitor);

    // todo подумать над отказом
    private sealed class InnerLobbyManagerVisitor(Lobby lobby) : ILobbyEventVisitor<bool>
    {
        public bool Visit(LobbyCreatedEvent e, CancellationToken ct = default)
        {
            lobby.InitContext(e.LobbySettings);
            return true;
        }

        public bool Visit(LobbyStatusChangedEvent e, CancellationToken ct = default)
        {
            lobby.Status = e.Status;
            return true;
        }

        public bool Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new AddPlayerRequest(e.UserId));
            return true;
        }

        public bool Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new RemovePlayerRequest(e.UserId));
            return true;
        }
    }
}



