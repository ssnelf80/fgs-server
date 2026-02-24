using FGS.Domain.FgsLobby.Context.Requests;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Context.States;

public class LobbyWelcomeState : LobbyState
{
    public override LobbyGameStateEnum GameState => LobbyGameStateEnum.GameWelcomeInformation;
    protected sealed override void DoBotActions()
    {
        foreach (var bot in BotPlayers())
        {
            _playerConfirmations.Add(bot.UserId);
        }
    }

    protected override string[] GetRandomPlayerChoice(Guid userId) => [string.Empty];

    private readonly HashSet<Guid> _playerConfirmations = [];

    public LobbyWelcomeState(LobbyState other) : base(other)
    {
        DoBotActions();
    }

    public override void Handle(ILobbyContextRequest request)
    {
        if (request is SetUserChoicesRequest userValueRequest)
        {
            if (!IsPlayerExists(userValueRequest.UserId))
                throw new LobbyStateException($"player with id = {userValueRequest.UserId} is not exist");
            
            if (userValueRequest.Choices.Length == 0)
                _playerConfirmations.Remove(userValueRequest.UserId);
            else
                _playerConfirmations.Add(userValueRequest.UserId);
            
            // todo next transition
            
            return;
        }
        base.Handle(request);
    }
}