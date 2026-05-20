using FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;
using FGS.Domain.FgsLobby.Context.GameSettings.Vote;

namespace FGS.Domain.FgsLobby.Context.GameSettings;

public interface ILobbyGameSettings;

public interface IVoteSettings : ILobbyGameSettings
{
    VoteGameSettings VoteGameSettings { get; }
}

public interface IRockPaperScissorsSettings : ILobbyGameSettings
{
    RockPaperScissorsSettings RockPaperScissorsSettings { get; }
}