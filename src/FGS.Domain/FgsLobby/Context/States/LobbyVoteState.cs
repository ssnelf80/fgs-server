using FGS.Domain.FgsLobby.Context.GameSettings;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyVoteState : LobbyState
{
    public LobbyVoteState(LobbyState other, IVoteSettings voteSettings) : base(other)
    {
    }

    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.InGame;
    public LobbyGameType GameType => LobbyGameType.Vote;
    
}