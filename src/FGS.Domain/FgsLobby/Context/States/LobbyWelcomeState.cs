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
        if (request is SetUserChoicesRequest userChoiceRequest)
        {
            if (!IsPlayerExists(userChoiceRequest.UserId))
                throw new LobbyStateException($"player with id = {userChoiceRequest.UserId} is not exist");
            
            if (userChoiceRequest.Choices.Length == 0)
                _playerConfirmations.Remove(userChoiceRequest.UserId);
            else
                _playerConfirmations.Add(userChoiceRequest.UserId);
            
            // todo next transition
            
            return;
        }
        
        if (request is SetRandomUserChoicesRequest randomChoiceRequest)
        {
            if (!IsPlayerExists(randomChoiceRequest.UserId))
                throw new LobbyStateException($"player with id = {randomChoiceRequest.UserId} is not exist");
            if (!GetPlayer(randomChoiceRequest.UserId).IsBot)
                throw new LobbyStateException($"player with id = {randomChoiceRequest.UserId} is not a bot");
            
            _playerConfirmations.Add(randomChoiceRequest.UserId);
            
            // todo next transition
            
            return;
        }
        base.Handle(request);
    }
}