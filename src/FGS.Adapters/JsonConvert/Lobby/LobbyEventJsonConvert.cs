using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;

namespace FGS.Adapters.JsonConvert.Lobby;

public class LobbyEventJsonConvert : ILobbyEventJsonConvert
{
    public byte[] Serialize(LobbyEvent e) => e.Accept(LobbyEventJsonSerializeToByteArrayVisitor.Instance);

    public string SerializeToString(LobbyEvent e) => e.Accept(LobbyEventJsonSerializeToStringVisitor.Instance);
}