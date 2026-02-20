using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Services;

public partial class LobbyGameManager
{
    private readonly LobbySettings _lobbySettings;
    public LobbyGameState LobbyGameState { get; private set; }
    private Random Random { get; }
    
    private readonly Dictionary<Guid, Player> _playersMap = [];
    private IReadOnlyList<Player> Players()  => _playersMap.Values.OrderBy(x=> x.UserId).ToList().AsReadOnly();
    
    private IReadOnlyList<Player> InnocentPlayers() => _playersMap.Values.Where(x=> x.Role == PlayerRole.Innocent).OrderBy(x=> x.UserId).ToList().AsReadOnly();

    private Guid GetRandomPlayerUserId(IReadOnlyList<Player> players) => players[Random.Next(0, players.Count)].UserId;
    
    public LobbyGameManager(LobbySettings settings)
    {
        ValidateLobbySettingsOrThrow(settings);
        _lobbySettings = settings;
        LobbyGameState = LobbyGameState.WaitPlayers;
        Random = new Random(settings.RandomizerSeed);
    }
}