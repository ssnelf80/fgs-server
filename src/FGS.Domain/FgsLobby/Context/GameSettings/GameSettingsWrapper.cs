using FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;
using FGS.Domain.FgsLobby.Context.GameSettings.Vote;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.GameSettings;

public class GameSettingsWrapper : IVoteSettings, IRockPaperScissorsSettings
{
    public required LobbyGameType LobbyGameType { get; init; }
    public VoteGameSettings VoteGameSettings { get; init; } = null!;
    public RockPaperScissorsSettings RockPaperScissorsSettings { get; init; } = null!;
}