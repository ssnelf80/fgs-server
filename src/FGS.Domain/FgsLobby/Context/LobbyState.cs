using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Context.States;
using FGS.Domain.FgsLobby.Context.States.Vote;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context;

public abstract class LobbyState
{
    private readonly LobbyState? _previousState = null;
    private readonly bool _isChildState = false;
    private LobbyStateContext? _context;
    protected LobbyStateContext Context => _context ?? throw new InvalidInnerCallLobbyStateException("Context is not initialized");
    
    protected abstract LobbyGameStateTypeEnum GameState { get; }
    public LobbyGameStateTypeEnum GetGameState()
    {
        if (!_isChildState)
            return GameState;

        if (_previousState is null)
            throw new InvalidOperationLobbyStateException("Previous state not initialized");
        return _previousState.GetGameState();
    }

    private const int GamesNotStarted = -1;
    private int _currentGameNumber = GamesNotStarted;
    protected int CurrentGameNumber => _currentGameNumber;
    private readonly bool _enableUnsafeContext = false;
    protected readonly LobbySettings LobbySettings;
    private readonly Dictionary<Guid, Player> _playersMap = [];
    private Random? _rnd = null;
    protected Random Random => _rnd ?? throw new InvalidInnerCallLobbyStateException("Lobby rand generator is not initialized");


    protected LobbyState(LobbyState other, bool isChildState) : this(other.LobbySettings, other._playersMap)
    {
        if (isChildState)
        {
            _isChildState = true;
            _previousState = other;
        }
        
        _context = other.Context;
        _rnd = other._rnd;
        _currentGameNumber = other._currentGameNumber;
    }
    
    protected LobbyState(
        LobbySettings lobbySettings, 
        Dictionary<Guid, Player> playersMap,
        bool enableUnsafeContext = false)
    {
        _enableUnsafeContext = enableUnsafeContext;
        LobbySettings = lobbySettings;
        _playersMap = playersMap;
    }

    /// <summary>
    /// Should be empty if the state has no actions!
    /// When a user is assigned as a bot, this method is called
    /// </summary>
    protected virtual void DoBotActions()
    {
        
    }
   
    protected bool IsPlayerExists(Guid userId) => _playersMap.ContainsKey(userId);
    protected Player GetPlayer(Guid playerId) => _playersMap[playerId];
    protected void UpdatePlayer(Player player)
    {
        if (!_playersMap.ContainsKey(player.UserId))
            throw new InvalidOperationLobbyStateException("Player not found");
        
        _playersMap[player.UserId] = player;
    }
    protected Dictionary<Guid, Player> UnsafePlayerMap => _enableUnsafeContext 
        ? _playersMap : throw new InvalidInnerCallLobbyStateException(nameof(UnsafePlayerMap));

    protected IReadOnlyList<Player> Players()  => _playersMap.Values.OrderBy(x=> x.UserId).ToList().AsReadOnly();
    protected IReadOnlyList<Player> InnocentPlayers() => _playersMap.Values.Where(x=> x.Role == PlayerRole.Innocent).OrderBy(x=> x.UserId).ToList().AsReadOnly();
    protected IReadOnlyList<Player> BotPlayers() => _playersMap.Values.Where(x => x.IsBot).OrderBy(x => x.UserId).ToList().AsReadOnly();
    protected Player GetRandomPlayer(IReadOnlyList<Player> players) => GetRandomItem(players);

    protected T GetRandomItem<T>(IReadOnlyList<T> items) => items[Random.Next(0, items.Count)];

    protected LobbyState GetNextGameState()
    {
        if (_isChildState)
        {
            if (_previousState is null)
                throw new InvalidOperationLobbyStateException("Previous state not initialized");
            
            _previousState.ApplyCallbackMessage(GetLobbyCallbackMessage());
            return _previousState;
        }
        
        if (LobbySettings.GamesSettings.Count == 0)
            return new LobbyEndState(this);
        
        _currentGameNumber++;
        if (_currentGameNumber < LobbySettings.GamesSettings.Count)
        {
            switch (LobbySettings.GamesSettings[_currentGameNumber].LobbyGameType)
            {
                case LobbyGameType.Vote:
                    return new LobbyVoteState(this, LobbySettings.GamesSettings[_currentGameNumber].VoteGameSettings, false);
                case LobbyGameType.RockPaperScissors:
                    return new LobbyRockPaperScissorsState(this,
                        LobbySettings.GamesSettings[_currentGameNumber].RockPaperScissorsSettings);
                default: // todo поддержку остальных игр
                    throw new InvalidOperationLobbyStateException("Game type not supported");
            }
        }
        
        return new LobbyEndState(this);
    }

    public abstract PlayerStateWrapper GetPlayerGameState(Guid userId);

    protected void ChangeBalance(Guid userId, BalanceOperation balanceOperation)
    {
        if (balanceOperation.Type == BalanceOperationType.NoAction)
            return;
        if (balanceOperation is { Type: BalanceOperationType.Division, Value: 0 })
            throw new InvalidOperationLobbyStateException("Cannot change balance. division by 0");
        var player = GetPlayer(userId);
        switch (balanceOperation.Type)
        {
            case BalanceOperationType.Addition:
                UpdatePlayer(player with { Balance = (long)(player.Balance + balanceOperation.Value) });
                break;
            case BalanceOperationType.Subtraction:
                UpdatePlayer(player with { Balance = (long)(player.Balance - balanceOperation.Value) });
                break;
            case BalanceOperationType.Multiplication:
                UpdatePlayer(player with { Balance = (long)(player.Balance * balanceOperation.Value) });
                break;
            case BalanceOperationType.Division:
                UpdatePlayer(player with { Balance = (long)(player.Balance / balanceOperation.Value) });
                break;
            default:
                throw new InvalidOperationLobbyStateException($"Balance operation type {balanceOperation.Type} not supported");
        }
        if (GetPlayer(userId).Balance < 0)
            UpdatePlayer(player with { Balance = 0 });
    }

    protected void InitRandom(int seed)
    {
        if (_rnd is not null)
            throw new InvalidInnerCallLobbyStateException("Rand generator already initialized");
        _rnd = new Random(seed);
    }

    public void SetContext(LobbyStateContext context) => _context = context;

    public virtual void Handle(ILobbyContextRequest request)
    {
        switch (request)
        {
            case SetBotToPlayerRequest requestSetBotToPlayer when !IsPlayerExists(requestSetBotToPlayer.UserId):
                throw new InvalidOperationLobbyStateException("Player not found");
            case SetBotToPlayerRequest requestSetBotToPlayer when GetPlayer(requestSetBotToPlayer.UserId).IsBot:
                throw new InvalidOperationLobbyStateException("Player is bot already");
            case SetBotToPlayerRequest requestSetBotToPlayer:
            {
                var player = GetPlayer(requestSetBotToPlayer.UserId) with { IsBot = true };
                UpdatePlayer(player);
                DoBotActions();
                return;
            }
            case RemoveBotFromPlayerRequest requestRemoveBotFromPlayer when !IsPlayerExists(requestRemoveBotFromPlayer.UserId):
                throw new InvalidOperationLobbyStateException("Player not found");
            case RemoveBotFromPlayerRequest requestRemoveBotFromPlayer when !GetPlayer(requestRemoveBotFromPlayer.UserId).IsBot:
                throw new InvalidOperationLobbyStateException("Player is not a bot");
            case RemoveBotFromPlayerRequest requestRemoveBotFromPlayer:
            {
                var player = GetPlayer(requestRemoveBotFromPlayer.UserId) with { IsBot = false };
                UpdatePlayer(player);
                return;
            }
            default:
                throw new InvalidOperationLobbyStateException(request.GetType().Name);
        }
    }

    protected virtual IStateCallbackMessage GetLobbyCallbackMessage() => EmptyStateCallbackMessage.Instance;

    protected virtual void ApplyCallbackMessage(IStateCallbackMessage message)
    {
        if (message is NotReadyStateCallbackMessage)
            throw new InvalidOperationLobbyStateException("Callback message is not ready");
    }
}