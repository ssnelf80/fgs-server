namespace FGS.Domain.FgsLobby.Context.States.Vote;

public record VoteStateCallbackMessage(IReadOnlyDictionary<Guid, int> VoteResult) : IStateCallbackMessage;