using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyWaitPlayerConnectionState : LobbyState
{
    public LobbyWaitPlayerConnectionState(LobbySettings lobbySettings, Dictionary<Guid, Player> playersMap) : base(lobbySettings, playersMap, true)
    {
        ValidateLobbySettingsOrThrow(lobbySettings);
        InitRandom(lobbySettings.RandomizerSeed);
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.WaitPlayers;

    public override PlayerGameState GetPlayerGameState(Guid userId)
    {
        var player = GetPlayer(userId);
        return new PlayerGameState
        {
            Balance = player.Balance,
            PlayerRole = player.Role,
            GameState = GameState,
            InnerGameState = string.Empty,
            GameNumber = CurrentGameNumber,
            Choices = ConfirmationChoice,
            SelectedChoices = ConfirmationChoice,
            CanSendChoice = true,
            GameInfoMessage = $"Ожидание других игроков: {Players().Count}/{LobbySettings.PlayersCount}",
            RoundInfoMessage = string.Join(",", Players().Select(x=> x.UserId))
        };
    }

    public override void Handle(ILobbyContextRequest request)
    {
        switch (request)
        {
            case AddPlayerRequest addPlayerRequest:
                AddPlayer(addPlayerRequest.UserId, addPlayerRequest.IsBot);
                return;
            case RemovePlayerRequest removePlayerRequest:
                RemovePlayer(removePlayerRequest.UserId);
                return;
            default:
                base.Handle(request);
                break;
        }
    }

    private void AddPlayer(Guid userId, bool isBot = false)
    {
        if (UnsafePlayerMap.ContainsKey(userId))
            throw new LobbyStateException($"Игрок с ID : {userId} уже существует");

        UnsafePlayerMap.Add(userId, new Player(userId, 0, PlayerRole.Unknown, isBot));

        if (UnsafePlayerMap.Count == LobbySettings.PlayersCount)
        {
            Context.TransitionTo(new LobbyInitializeState(this));
            Context.SendRequest(new InitializeGameRequest());
        }
           
    }
    private void RemovePlayer(Guid userId)
    {
        if (!UnsafePlayerMap.Remove(userId))
            throw new LobbyStateException($"Игрок с ID : {userId} не существует");
    }
    
    private void ValidateLobbySettingsOrThrow(LobbySettings s)
    {
        if (s.PlayersCount < 2)
            throw new LobbyStateException($"Минимальное кол-во игроков для создания лобби: 2. Указано: {s.PlayersCount}");
        if (s.TraitorsCount > s.PlayersCount)
            throw new LobbyStateException($"Кол-во предателей меньше кол-ва игроков. Указано {s.TraitorsCount}/{s.PlayersCount} ");
        if (s.StartBalance < 0)
            throw new LobbyStateException($"Стартовый баланс игроков < 0 {s.StartBalance} ");
    }
    
    
}