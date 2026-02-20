using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Services;

public class LobbyGameManager
{
    public LobbySettings LobbySettings { get; }
    public LobbyGameState LobbyGameState { get; private set; }
    public Random Random { get; }
    
    private Dictionary<Guid, Player> _playersMap = [];
    public IReadOnlyList<Player> Players()  => _playersMap.Values.OrderBy(x=> x.UserId).ToList().AsReadOnly();
    public IReadOnlyList<Player> InnocentPlayers() => _playersMap.Values.Where(x=> x.Role == PlayerRole.Innocent).OrderBy(x=> x.UserId).ToList().AsReadOnly();

    public LobbyGameManager(LobbySettings settings)
    {
        ValidateLobbySettingsOrThrow(settings);
        LobbySettings = settings;
        LobbyGameState = LobbyGameState.WaitPlayers;
        Random = new Random(settings.RandomizerSeed);
    }

    public bool ReadyToStartInitialize() =>
        LobbyGameState == LobbyGameState.WaitPlayers && _playersMap.Count == LobbySettings.PlayersCount;
    public void AddPlayer(Guid userId)
    {
        if (LobbyGameState != LobbyGameState.WaitPlayers)
            throw new LobbyGameManagerException($"Нельзя добавить игроков. LobbyGameState = {LobbyGameState}");
        
        if (_playersMap.ContainsKey(userId))
            throw new LobbyGameManagerException($"Игрок с ID : {userId} уже существует");

        _playersMap.Add(userId, new Player(userId, LobbySettings.StartBalance, PlayerRole.Unknown, false));
    }
    public void RemovePlayer(Guid userId)
    {
        if (LobbyGameState != LobbyGameState.WaitPlayers)
            throw new LobbyGameManagerException($"Нельзя исключить игроков. LobbyGameState = {LobbyGameState}");
        
        if (!_playersMap.Remove(userId))
            throw new LobbyGameManagerException($"Игрок с ID : {userId} не существует");
    }
    public void InitializeStartState()
    {
        if (!ReadyToStartInitialize())
            throw new LobbyGameManagerException($"Нельзя инициализировать игру. LobbyGameState = {LobbyGameState}, PlayersCount = {_playersMap.Count} {LobbySettings.PlayersCount}");

        LobbyGameState = LobbyGameState.Initialize;
        
        InitPlayerRoles();
        InitStartBalance();
    }
    private void InitStartBalance()
    {
        foreach (var uid in _playersMap.Keys)
            _playersMap[uid] = _playersMap[uid] with { Balance = LobbySettings.StartBalance };
    }
    private void InitPlayerRoles()
    {
        foreach (var uid in _playersMap.Keys)
            _playersMap[uid] = _playersMap[uid] with { Role = PlayerRole.Innocent };

        for (var i = 0; i < LobbySettings.TraitorsCount; i++)
        {
            var rndPlayerId = GetRandomPlayerUserId(InnocentPlayers());
            _playersMap[rndPlayerId] = _playersMap[rndPlayerId] with { Role = PlayerRole.Traitor };
        }
    }
    private Guid GetRandomPlayerUserId(IReadOnlyList<Player> players) => players[Random.Next(0, players.Count)].UserId;
    private void ValidateLobbySettingsOrThrow(LobbySettings s)
    {
        if (s.PlayersCount < 2)
            throw new LobbyGameManagerException($"Минимальное кол-во игроков для создания лобби: 2. Указано: {s.PlayersCount}");
        if (s.MasterUserId == Guid.Empty)
            throw new LobbyGameManagerException("Id мастера игры не определено.(MasterUserId == default)");
        if (s.TraitorsCount > s.PlayersCount)
            throw new LobbyGameManagerException($"Кол-во предателей меньше кол-ва игроков. Указано {s.TraitorsCount}/{s.PlayersCount} ");
        if (s.StartBalance < 0)
            throw new LobbyGameManagerException($"Стартовый баланс игроков < 0 {s.StartBalance} ");
    }
}