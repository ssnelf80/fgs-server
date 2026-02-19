using FGS.Auth.Entities;
using FGS.Auth.Enums;
using FGS.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGS.Host.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class UserController(SignInManager<FgsUser> signInManager, FgsUserService fgsUserService) : ControllerBase
{
    [HttpGet]
    [Route("current/roles")]
    public async Task<IList<string>> UserRolesListAsync(CancellationToken cancellationToken)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        return await signInManager.UserManager.GetRolesAsync(user!);
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