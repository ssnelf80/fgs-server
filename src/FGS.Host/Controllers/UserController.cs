using FGS.Auth.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGS.Host.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class UserController(SignInManager<FgsUser> signInManager) : ControllerBase
{
    [HttpGet]
    [Route("current/roles")]
    public async Task<IList<string>> UserRolesListAsync(CancellationToken cancellationToken)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);
        return await signInManager.UserManager.GetRolesAsync(user!);
    }
}