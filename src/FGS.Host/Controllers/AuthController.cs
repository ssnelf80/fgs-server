using FGS.Auth.Entities;
using FGS.Auth.Managers;
using FGS.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace FGS.Host.Controllers;

[ApiController]
[Route("/")]
public class AuthController(SignInManager<FgsUser> signInManager, FgsUserManager fgsUserManager) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IdentityResult> CreateUserAsync(LoginModel request, CancellationToken cancellationToken)
    {
        return await fgsUserManager.CreateDefaultUser(request.Login, request.Password);
    }

    [HttpPost]
    [Route("login")]
    public async Task LoginAsync(LoginModel request, CancellationToken cancellationToken)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.Login, 
            request.Password, 
            true, 
            false);

        if (!result.Succeeded) 
            HttpContext.Response.StatusCode = 401;
    }
    
    [HttpPost]
    [Authorize]
    [Route("logout")]
    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
    }
}