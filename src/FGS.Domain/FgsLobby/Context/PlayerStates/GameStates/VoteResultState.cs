namespace FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;

public record VoteResultState(bool IsConfirmed, IDictionary<Guid, int> VoteResult, string Description) : ConfirmationState(IsConfirmed)
{
    public new PlayerGameStateTypeEnum PlayerGameStateTypeEnum => PlayerGameStateTypeEnum.VoteResult;
}