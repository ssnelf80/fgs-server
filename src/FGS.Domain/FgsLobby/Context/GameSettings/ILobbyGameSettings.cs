namespace FGS.Domain.FgsLobby.Context.GameSettings;

public interface ILobbyGameSettings;

public interface IVoteSettings : ILobbyGameSettings
{
    VoteGameSettings  VoteGameSettings { get; }
}