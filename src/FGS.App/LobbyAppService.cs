using FGS.App.Models;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Events;

namespace FGS.App;

public class LobbyAppService(IAggregateRepository<Lobby, LobbyEvent> lobbyRepository)
{
    public async Task CreateLobbyAsync(CreateLobbyRequest request, CancellationToken ct)
    {
        // todo оплетку над стандартными исключениями eventStore и аггрегатов
        var lobby = Lobby.Create(request.UserId, request.Name, LobbySettings.Default);
        await lobbyRepository.SaveAsync(lobby, ct);
    }
}