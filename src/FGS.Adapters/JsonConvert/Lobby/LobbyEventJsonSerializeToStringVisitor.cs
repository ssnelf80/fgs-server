using System.Text.Json;
using FGS.Domain.FgsLobby.Events;

namespace FGS.Adapters.JsonConvert.Lobby;

public class LobbyEventJsonSerializeToStringVisitor : ILobbyEventVisitor<string>
{
    public static LobbyEventJsonSerializeToStringVisitor Instance => new();
    
    public string Visit(LobbyCreatedEvent e, CancellationToken ct = default) => JsonSerializer.Serialize(e);

    public string Visit(LobbyStatusChangedEvent e, CancellationToken ct = default) => JsonSerializer.Serialize(e);

    public string Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default) => JsonSerializer.Serialize(e);

    public string Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default) => JsonSerializer.Serialize(e);
    public string Visit(LobbySetUserChoiceEvent e, CancellationToken ct = default)
        => JsonSerializer.Serialize(e);

    public string Visit(LobbySetUserRandomChoiceEvent e, CancellationToken ct = default)
        => JsonSerializer.Serialize(e);

    public string Visit(SetBotToPlayerLobbyEvent e, CancellationToken ct = default)
        => JsonSerializer.Serialize(e);

    public string Visit(RemoveBotFromPlayerLobbyEvent e, CancellationToken ct = default)
        => JsonSerializer.Serialize(e);
}