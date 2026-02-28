using FGS.App;
using FGS.Auth.Entities;
using FGS.Auth.Enums;
using FGS.Auth.Managers;
using FGS.Auth.Models;
using FGS.Auth.Services;
using FGS.Domain.FgsLobby.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGS.Host.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class UserController(
    SignInManager<FgsUser> signInManager, 
    FgsUserService fgsUserService,
    UserConnectionService userConnectionService
    ) : ControllerBase
{
    [HttpGet]
    [Route("current/roles")]
    public async Task<IList<string>> UserRolesListAsync(CancellationToken cancellationToken)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        return await signInManager.UserManager.GetRolesAsync(user!);
    }
    
    [HttpGet]
    [Route("current")]
    public async Task<UserInfoModelWithRoles> GetUserInfoWithRolesAsync(CancellationToken cancellationToken)
    {
        var manager = (FgsUserManager)signInManager.UserManager;
        var user = await manager.GetUserAsync(User);
        return await manager.GetUserInfoWithRolesAsync(user!);
    }
    
    [HttpGet]
    [Route("connection/current")]
    public async Task<ConnectionTrackerEntity?> GetCurrentUserConnectionOrDefaultAsync(CancellationToken cancellationToken)
    {
        var manager = (FgsUserManager)signInManager.UserManager;
        var user = await manager.GetUserAsync(User);
        return await userConnectionService.GetConnectionTrackerOrDefault(Guid.Parse(user!.Id), cancellationToken);
    }

    [HttpGet]
    [Authorize(Roles = nameof(FgsUserRole.Admin))]
    [Route("list")]
    public async Task<IReadOnlyCollection<UserEntity>> UsersListAsync(
        string? searchString, 
        int? offset, 
        int? limit,
        bool? ingnoreCase, 
        CancellationToken cancellationToken)
    {
        return await fgsUserService.GetUsersAsync(new FgsUserEntitySearchFilter
        {
            Offset = offset ?? 0,
            Limit = limit ?? 20,
            IgnoreCase = ingnoreCase ?? true,
            SearchString = searchString ?? string.Empty,
        }, cancellationToken);
    }
}