using FGS.Domain.FgsLobby.Events;

namespace FGS.Domain.Services;

public interface ILobbyEventJsonConvert
{
    public byte[] Serialize(LobbyEvent e);
    public string SerializeToString(LobbyEvent e);
}