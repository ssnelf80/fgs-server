namespace FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;

public record ConfirmationState(bool IsConfirmed) : IGameState
{
    public PlayerGameStateTypeEnum PlayerGameStateTypeEnum => PlayerGameStateTypeEnum.Confirmation;
    // todo {nazarov} либо отдельный тип, либо расширить класс для возможности показа результата
}