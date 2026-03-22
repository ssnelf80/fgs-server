using System.Text.Json.Serialization;

namespace FGS.Domain.FgsLobby.Context.PlayerStates.GameStates;

[JsonDerivedType(typeof(ConfirmationState))]
[JsonDerivedType(typeof(VoteState))]
[JsonDerivedType(typeof(EmptyState))]
[JsonDerivedType(typeof(WaitPlayerConnectionState))]
public interface IGameState
{
    PlayerGameStateTypeEnum PlayerGameStateTypeEnum { get; } 
}