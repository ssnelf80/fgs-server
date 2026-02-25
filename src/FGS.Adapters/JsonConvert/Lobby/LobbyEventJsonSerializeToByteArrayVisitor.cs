using System.Text.Json;
using FGS.Domain.FgsLobby.Events;

namespace FGS.Adapters.JsonConvert.Lobby;

public class LobbyEventJsonSerializeToByteArrayVisitor : ILobbyEventVisitor<byte[]>
{
    public static LobbyEventJsonSerializeToByteArrayVisitor Instance => new();
    
    public byte[] Visit(LobbyCreatedEvent e, CancellationToken ct = default) 
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(LobbyStatusChangedEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(PlayerConnectedLobbyEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(PlayerDisconnectedLobbyEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(LobbySetUserChoiceEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(LobbySetUserRandomChoiceEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(SetBotToPlayerLobbyEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);

    public byte[] Visit(RemoveBotFromPlayerLobbyEvent e, CancellationToken ct = default)
        => JsonSerializer.SerializeToUtf8Bytes(e);
}