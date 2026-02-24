using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Context;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Aggregate;

public sealed partial class Lobby : AggregateRoot<LobbyEvent>
{
    private LobbyStateContext _context = null!;
    private LobbyStateContext Context => _context ?? throw new LobbyException("LobbyContext is not initialized");
    private readonly InnerLobbyManagerVisitor _innerLobbyManagerVisitor;
    public LobbyStatus Status { get; private set; }
    
    public Lobby(Guid id, ulong version, IReadOnlyCollection<LobbyEvent> commitedEvents) : base(id, version)
    {
        _innerLobbyManagerVisitor = new InnerLobbyManagerVisitor(this);
        foreach (var e in commitedEvents)
            ApplyChanges(e);
        CommitEvents();
    }

    public static Lobby Create(Guid masterUserId, string name, LobbySettings lobbySettings)
    {
        var lobby = new Lobby(Guid.NewGuid(), NoVersion, []);
        lobby.EmitEvent(new LobbyCreatedEvent(lobby.Id, name, masterUserId, lobbySettings, DateTimeOffset.UtcNow));
        return lobby;
    }

    public void ConnectBot(Guid botId)
    {
        EmitEvent(new BotConnectedLobbyEvent(Id, botId));
        
        if (Context.Status != LobbyGameStateEnum.WaitPlayers)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.InProgress));
        if (Context.Status == LobbyGameStateEnum.EndState)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.Closed));
    }
    
    public void DisconnectBot(Guid botId) => EmitEvent(new BotDisconnectedLobbyEvent(Id, botId));

    public void SetBotToUser(Guid userId)
    {
        EmitEvent(new SetBotToPlayerLobbyEvent(Id, userId));
        if (Context.Status == LobbyGameStateEnum.EndState)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.Closed));
    }

    public void RemoveBotFromUser(Guid userId)
    {
        EmitEvent(new RemoveBotFromPlayerLobbyEvent(Id, userId));
    }

    public void SetUserChoice(Guid userId, params string[] choices)
    {
        EmitEvent(new LobbySetUserChoiceEvent(Id, userId, choices));
        if (Context.Status == LobbyGameStateEnum.EndState)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.Closed));
    }
    
    public void SetRandomUserChoice(Guid userId)
    {
        EmitEvent(new LobbySetUserRandomChoiceEvent(Id, userId));
        if (Context.Status == LobbyGameStateEnum.EndState)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.Closed));
    }
    
    public void DisconnectUser(Guid userId) => EmitEvent(new PlayerDisconnectedLobbyEvent(Id, userId));

    public void ConnectUser(Guid userId)
    {
        EmitEvent(new PlayerConnectedLobbyEvent(Id, userId));
        
        if (Context.Status != LobbyGameStateEnum.WaitPlayers)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.InProgress));
        if (Context.Status == LobbyGameStateEnum.EndState)
            EmitEvent(new LobbyStatusChangedEvent(Id, LobbyStatus.Closed));
    }

    private void InitContext(LobbySettings settings)
    {
        if (_context is not null)
            throw new LobbyException("LobbyContext already initialized");
        _context = LobbyStateContext.Create(settings);
    }

    protected override void ApplyChanges(LobbyEvent e) => e.Accept(_innerLobbyManagerVisitor);
}





