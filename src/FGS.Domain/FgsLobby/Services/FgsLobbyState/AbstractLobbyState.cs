using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Services.FgsLobbyState;

public abstract class AbstractLobbyState
{
    private LobbyStateContext _context = null!;
    protected LobbyStateContext Context => _context ?? throw new LobbyGameManagerException("Lobby state context is not initialized");
    
    public abstract LobbyGameStateEnum GameState { get; }
    protected readonly LobbySettings LobbySettings;
    protected readonly Dictionary<Guid, Player> PlayersMap = [];
    private Random? _rnd = null;
    protected Random Random => _rnd ?? throw new LobbyGameManagerException("Lobby rand generator is not initialized");
    
   
    protected IReadOnlyList<Player> Players()  => PlayersMap.Values.OrderBy(x=> x.UserId).ToList().AsReadOnly();
    
    protected IReadOnlyList<Player> InnocentPlayers() => PlayersMap.Values.Where(x=> x.Role == PlayerRole.Innocent).OrderBy(x=> x.UserId).ToList().AsReadOnly();

    protected Guid GetRandomPlayerUserId(IReadOnlyList<Player> players) => players[Random.Next(0, players.Count)].UserId;

    protected AbstractLobbyState(AbstractLobbyState other) : this(other.LobbySettings, other.PlayersMap)
    {
        
    }
    protected AbstractLobbyState(LobbySettings lobbySettings, Dictionary<Guid, Player> playersMap)
    {
        LobbySettings = lobbySettings;
        PlayersMap = playersMap;
    }

    protected void InitRandom(int seed)
    {
        if (_rnd is not null)
            throw new LobbyGameManagerException("Rand generator already initialized");
        _rnd = new Random(seed);
    }

    public void SetContext(LobbyStateContext context) => _context = context;

    public virtual void Handle(ILobbyStateRequest request)
    {
        // todo подробности исключения
        throw new LobbyGameManagerException("Недопустимая операция");
    }
}