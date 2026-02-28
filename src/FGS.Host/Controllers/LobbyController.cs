using FGS.App;
using FGS.App.Models;
using FGS.Auth.Entities;
using FGS.Auth.Enums;
using FGS.Domain.FgsLobby.Context.PlayerStates;
using FGS.Domain.FgsLobby.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGS.Host.Controllers;

[ApiController]
[Authorize]
[Route("lobby")]
public class LobbyController(SignInManager<FgsUser> signInManager, LobbyAppService lobbyAppService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(FgsUserRole.Master))]
    [Route("create")]
    public async Task CreateLobbyAsync(CreateLobbyRequest request, CancellationToken ct)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        await lobbyAppService.CreateLobbyAsync(request, ct);
    }

    [HttpGet]
    [Route("{lobbyId}/game-state")]
    public async Task<PlayerGameState> GetPlayerGameState(Guid lobbyId, CancellationToken cancellationToken)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        return await lobbyAppService.GetPlayerGameStateAsync(lobbyId, Guid.Parse(user!.Id), cancellationToken);
    }
    
    [HttpPost]
    [Route("{lobbyId}/send-choices")]
    [Authorize(Roles = nameof(FgsUserRole.Player))]
    public async Task GetPlayerGameState(Guid lobbyId, string[] choices, CancellationToken cancellationToken)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        await lobbyAppService.SendChoicesToLobbyAsync(lobbyId, Guid.Parse(user!.Id), choices, cancellationToken);
    }
    
    [HttpPost]
    [Authorize(Roles = nameof(FgsUserRole.Master))]
    [Route("{lobbyId}/add-bot")]
    public Task AddBotAsync(Guid lobbyId, CancellationToken cancellationToken) =>
        // todo проверка, что вызов от хозяина-лобби
        lobbyAppService.AddBotToLobbyAsync(lobbyId, cancellationToken);

    [HttpGet]
    [Route("list")]
    public async Task<LobbyEntityWithUserList> UsersListAsync(
        string? searchString, 
        int? offset, 
        int? limit,
        bool? ingnoreCase, 
        Guid? strictLobbyId,
        CancellationToken cancellationToken)
    {
       return await lobbyAppService.GetLobbyListAsync(new LobbyEntitySearchFilter
        {
            Offset = offset ?? 0,
            Limit = limit ?? 20,
            IgnoreCase = ingnoreCase ?? true,
            SearchString = searchString ?? string.Empty,
            StrictLobbyId = strictLobbyId
        }, cancellationToken);
    }

    [HttpPost]
    [Route("{lobbyId}/connect")]
    public async Task CreateLobbyAsync(Guid lobbyId, CancellationToken ct)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        await lobbyAppService.ConnectUserToLobbyAsync(lobbyId, Guid.Parse(user!.Id), ct);
    }
}