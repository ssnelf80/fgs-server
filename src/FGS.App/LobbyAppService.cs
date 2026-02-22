using FGS.App.Models;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;

namespace FGS.App;

public class LobbyAppService(
    IAggregateRepository<Lobby, LobbyEvent> lobbyRepository,
    IFgsViewModelRepository fgsViewModelRepository
    )
{
    public async Task CreateLobbyAsync(CreateLobbyRequest request, CancellationToken ct)
    {
        // todo оплетку над стандартными исключениями eventStore и аггрегатов
        var lobby = Lobby.Create(request.UserId, request.Name, LobbySettings.Default);
        await lobbyRepository.SaveAsync(lobby, ct);
    }

    public Task<IReadOnlyCollection<LobbyEntity>> GetLobbyListAsync(LobbyEntitySearchFilter searchFilter,
        CancellationToken ct) =>
        fgsViewModelRepository.GetLobbyEntitiesListAsync(searchFilter, ct);
}