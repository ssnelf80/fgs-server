using FGS.App.Models;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;

namespace FGS.App;

public class LobbyAppService(
    IAggregateRepository<Lobby, LobbyEvent> lobbyRepository,
    IFgsViewModelRepository fgsViewModelRepository,
    IConnectionTrackerService connectionTrackerService
    )
{
    public async Task<Guid> CreateLobbyAsync(CreateLobbyRequest request, CancellationToken ct)
    {
        // todo оплетку над стандартными исключениями eventStore и аггрегатов
        var lobby = Lobby.Create(request.UserId, request.Name, LobbySettings.Default);
        await lobbyRepository.SaveAsync(lobby, ct);
        return lobby.Id;
    }

    public Task<IReadOnlyCollection<LobbyEntity>> GetLobbyListAsync(LobbyEntitySearchFilter searchFilter,
        CancellationToken ct) =>
        fgsViewModelRepository.GetLobbyEntitiesListAsync(searchFilter, ct);

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