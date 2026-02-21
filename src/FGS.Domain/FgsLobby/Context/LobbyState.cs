using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context;

public abstract class LobbyState
{
    private LobbyStateContext _context = null!;
    protected LobbyStateContext Context => _context ?? throw new LobbyStateException("Lobby state context is not initialized");
    
    public abstract LobbyGameStateEnum GameState { get; }
    
    private readonly bool _enableUnsafeContext = false;
    protected readonly LobbySettings LobbySettings;
    private readonly Dictionary<Guid, Player> _playersMap = [];
    private Random? _rnd = null;
    protected Random Random => _rnd ?? throw new LobbyStateException("Lobby rand generator is not initialized");
    
    protected LobbyState(LobbyState other) : this(other.LobbySettings, other._playersMap)
    {
        
    }
    protected LobbyState(LobbySettings lobbySettings, Dictionary<Guid, Player> playersMap, bool enableUnsafeContext = false)
    {
        _enableUnsafeContext = enableUnsafeContext;
        LobbySettings = lobbySettings;
        _playersMap = playersMap;
    }
    
    protected Player GetPlayer(Guid playerId) => _playersMap[playerId];
    protected Player UpdatePlayer(Player player)
    {
        if (!_playersMap.ContainsKey(player.UserId))
            throw new LobbyStateException("Player not found");
        
        _playersMap[player.UserId] = player;
        return player;
    }
    protected Dictionary<Guid, Player> UnsafePlayerMap => _enableUnsafeContext ? _playersMap : throw new LobbyStateException("Unsafe call");

    protected IReadOnlyList<Player> Players()  => _playersMap.Values.OrderBy(x=> x.UserId).ToList().AsReadOnly();
    protected IReadOnlyList<Player> InnocentPlayers() => _playersMap.Values.Where(x=> x.Role == PlayerRole.Innocent).OrderBy(x=> x.UserId).ToList().AsReadOnly();
    protected Player GetRandomPlayer(IReadOnlyList<Player> players) => players[Random.Next(0, players.Count)];

    protected void InitRandom(int seed)
    {
        if (_rnd is not null)
            throw new LobbyStateException("Rand generator already initialized");
        _rnd = new Random(seed);
    }

    public void SetContext(LobbyStateContext context) => _context = context;

    public virtual void Handle(ILobbyContextRequest request)
    {
        // todo подробности исключения
        throw new LobbyStateException("Недопустимая операция");
    }
}