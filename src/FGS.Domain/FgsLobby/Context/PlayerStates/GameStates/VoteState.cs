namespace FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;

public record VoteState : IGameState
{
    public PlayerGameStateTypeEnum PlayerGameStateTypeEnum => PlayerGameStateTypeEnum.Vote;
    public required bool CanMultiplyChoice { get; init; }
    public required bool CanSelfChoice { get; init; }
    public required IReadOnlyCollection<string> EnabledChoices { get; init; }
    public required IReadOnlyCollection<string> SelectedChoices { get; init; }  
    public required string? IndividualDescription { get; init; }
}