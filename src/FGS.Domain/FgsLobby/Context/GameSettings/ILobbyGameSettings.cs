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

public interface IGameSettings<TGameSettings, TPlayerSettings>
{
    public TGameSettings GlobalGameSettings { get; }
    public TPlayerSettings DefaultPlayerSettings { get; }
}