using FGS.App.Models;
using FGS.Auth.Services;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;

namespace FGS.App;

public class LobbyAppService(
    IAggregateRepository<Lobby, LobbyEvent> lobbyRepository,
    IFgsViewModelRepository fgsViewModelRepository,
    IConnectionTrackerService connectionTrackerService,
    FgsUserService fgsUserService
    )
{
    public async Task SendChoicesToLobbyAsync(Guid lobbyId, Guid userId, string[] choices, CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        lobby.SetUserChoice(userId, choices);
        await lobbyRepository.SaveAsync(lobby, ct);
    }
    
    public async Task AddBotToLobbyAsync(Guid lobbyId, CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        lobby.ConnectBot(Guid.NewGuid());
        await lobbyRepository.SaveAsync(lobby, ct);
    }
    
    public async Task<PlayerGameState> GetPlayerGameStateAsync(Guid lobbyId, Guid userId, CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        return lobby.GetPlayerGameState(userId);
    }
    
    public async Task<Guid> CreateLobbyAsync(CreateLobbyRequest request, CancellationToken ct)
    {
        // todo оплетку над стандартными исключениями eventStore и агрегатов
        var lobby = Lobby.Create(request.UserId, request.Name, LobbySettings.Default);
        await lobbyRepository.SaveAsync(lobby, ct);
        return lobby.Id;
    }

    public async Task<LobbyEntityWithUserList> GetLobbyListAsync(LobbyEntitySearchFilter searchFilter,
        CancellationToken ct)
    {
        var entities = await fgsViewModelRepository.GetLobbyEntitiesListAsync(searchFilter, ct);
        HashSet<Guid> userIds = entities.Select(x => x.MasterUserId).ToHashSet();
        userIds.UnionWith(entities.SelectMany(x => x.ConnectedUsers).Distinct());
        var userNames = await fgsUserService.GetUserNamesAsync(userIds, ct);
        return new LobbyEntityWithUserList(entities, userNames);
    }

    public async Task SendUserChoiceToLobbyAsync(Guid lobbyId, Guid userId, string[] values,
        CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        lobby.SetUserChoice(userId, values);
        await lobbyRepository.SaveAsync(lobby, ct);
    }
    
    public async Task SendRandomUserChoiceToLobbyAsync(Guid lobbyId, Guid userId, CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        lobby.SetRandomUserChoice(userId);
        await lobbyRepository.SaveAsync(lobby, ct);
    }

    public async Task ConnectBotToLobbyAsync(Guid lobbyId, CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        lobby.ConnectBot(Guid.NewGuid()); // todo список ботов
        await lobbyRepository.SaveAsync(lobby, ct);
    }
    
    public async Task DisconnectBotFromLobbyAsync(Guid lobbyId, Guid botId, CancellationToken ct)
    {
        throw new  NotImplementedException();
    }

    public async Task ConnectUserToLobbyAsync(Guid lobbyId, Guid userId, CancellationToken ct)
    {
        await connectionTrackerService.ConnectAsync(new ConnectionTrackerEntity(userId, lobbyId, LobbyUserRole.Player), ct);
        try
        {
            var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
            lobby.ConnectUser(userId);
            await lobbyRepository.SaveAsync(lobby, ct);
        }
        catch (Exception)
        {
            // todo wrap error
            await connectionTrackerService.DisconnectAsync(userId, ct);
        }
    }

    public async Task DisconnectUserFromLobbyAsync(Guid lobbyId, Guid userId, CancellationToken ct)
    {
        var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
        lobby.DisconnectUser(userId);
        await lobbyRepository.SaveAsync(lobby, ct);
        await connectionTrackerService.DisconnectAsync(userId, ct);
    }
}