using FGS.Auth.Entities;
using FGS.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGS.Host.Controllers;

[ApiController]
[Route("/")]
public class AuthController(SignInManager<FgsUser> signInManager) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task CreateUserAsync(LoginModel request, CancellationToken cancellationToken)
    {
       
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