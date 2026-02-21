using FGS.App;
using FGS.App.Models;
using FGS.Auth.Entities;
using FGS.Auth.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGS.Host.Controllers;

[ApiController]
[Authorize]
[Route("lobby")]
public class LobbyController(SignInManager<FgsUser> signInManager, LobbyAppService lobbyAppService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(FgsUserRole.Master))]
    [Route("create")]
    public async Task CreateLobbyAsync(CancellationToken ct)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        var userId = Guid.Parse(user!.Id);
        await lobbyAppService.CreateLobbyAsync(new CreateLobbyRequest(userId, Guid.NewGuid().ToString()), ct);
    }
}