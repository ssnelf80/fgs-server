using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;

namespace FGS.Domain.FgsLobby.Aggregate;

public sealed partial class Lobby
{
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
            if (lobby.Context.Status == LobbyGameStateEnum.ReadyToInitialize)
                lobby.Context.SendRequest(new InitializeGameRequest());
            return true;
        }

        public bool Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new RemovePlayerRequest(e.UserId));
            return true;
        }

        public bool Visit(LobbySetUserChoiceEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new SetUserChoicesRequest(e.UserId, e.Choices));
            return true;
        }

        public bool Visit(LobbySetUserRandomChoiceEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new SetRandomUserChoicesRequest(e.UserId));
            return true;
        }

        public bool Visit(BotConnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new AddPlayerRequest(e.BotId, true));
            if (lobby.Context.Status == LobbyGameStateEnum.ReadyToInitialize)
                lobby.Context.SendRequest(new InitializeGameRequest());
            return true;
        }

        public bool Visit(BotDisconnectedLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new RemovePlayerRequest(e.BotId));
            return true;
        }

        public bool Visit(SetBotToPlayerLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new SetBotToPlayerRequest(e.UserId));
            return true;
        }

        public bool Visit(RemoveBotFromPlayerLobbyEvent e, CancellationToken ct = default)
        {
            lobby.Context.SendRequest(new RemoveBotFromPlayerRequest(e.UserId));
            return true;
        }
    }
}