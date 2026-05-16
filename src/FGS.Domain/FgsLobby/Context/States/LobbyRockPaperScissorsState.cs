using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Context.GameSettings.RockPaperScissors;
using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Context.States.Base;
using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyRockPaperScissorsState(LobbyState other, RockPaperScissorsSettings gameSettings)
    : LobbyGameBase<RockPaperScissorsSettings, PlayerRockPaperScissorsSettings>(other, gameSettings)
{
    private const string Rock = "Rock";
    private const string Paper = "Paper";
    private const string Scissors = "Scissors";
    
    private Dictionary<Guid, IReadOnlyList<string>> UserChoicesHistory { get; init; }
    
    public override LobbyGameStateTypeEnum GameState => LobbyGameStateTypeEnum.RockPaperScissors;
    public override PlayerStateWrapper GetPlayerGameState(Guid userId)
    {
        throw new NotImplementedException();
    }

    protected override IReadOnlyList<string> GetUserRandomChoices(Guid userId)
    {
        throw new NotImplementedException();
    }

    protected override GameStatus SetResult()
    {
        throw new NotImplementedException();
    }

    protected override void RoundSetLocalSettings()
    {
        throw new NotImplementedException();
    }


    protected override ValidationResult Validate(SetUserChoicesRequest request)
    {
        throw new NotImplementedException();
    }
}