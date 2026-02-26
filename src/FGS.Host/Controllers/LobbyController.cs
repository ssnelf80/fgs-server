using FGS.App;
using FGS.App.Models;
using FGS.Auth.Entities;
using FGS.Auth.Enums;
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
    [Route("connect/{lobbyId}")]
    public async Task CreateLobbyAsync(Guid lobbyId, CancellationToken ct)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        await lobbyAppService.ConnectUserToLobbyAsync(lobbyId, Guid.Parse(user!.Id), ct);
    }
}