namespace FGS.Domain.FgsLobby.Context.GameSettings;

public interface ILobbyGameSettings;


public interface IVoteSettings : ILobbyGameSettings
{
    VoteGameSettings VoteGameSettings { get; }
}

public interface IGameSettings<T>
{
    public T GlobalGameSettings { get; }
    public IReadOnlyList<T> RandomIndividualGameSettings { get; }
}