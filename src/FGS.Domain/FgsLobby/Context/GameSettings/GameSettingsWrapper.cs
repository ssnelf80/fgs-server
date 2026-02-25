using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings;

public class GameSettingsWrapper : IVoteSettings
{
    public required LobbyGameType LobbyGameType { get; init; }
    public VoteGameSettings VoteGameSettings { get; init; } = null!;
}