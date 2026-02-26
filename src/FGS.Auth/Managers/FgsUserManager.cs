using FGS.Auth.Entities;
using FGS.Auth.Enums;
using FGS.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FGS.Auth.Managers;

public class FgsUserManager(
    IUserStore<FgsUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<FgsUser> passwordHasher,
    IEnumerable<IUserValidator<FgsUser>> userValidators,
    IEnumerable<IPasswordValidator<FgsUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<FgsUser>> logger)
    : UserManager<FgsUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
        errors, services, logger)
{
    public async Task<IdentityResult> CreateDefaultAdminAsync()
    {
        var user = new FgsUser("admin");
        var identityResult = await CreateAsync(user, "admin");
        
        if (!identityResult.Succeeded)
            return identityResult;
        
        return await AddToRolesAsync(user, 
        [nameof(FgsUserRole.Admin), 
            nameof(FgsUserRole.Player), 
            nameof(FgsUserRole.Master), 
            nameof(FgsUserRole.Spectator)]);
    }

    public async Task<IdentityResult> CreateDefaultUser(string login, string password)
    {
        var user = new FgsUser(login);
        var identityResult = await CreateAsync(user, password);
        
        if (!identityResult.Succeeded)
            return identityResult;
        
        return await AddToRolesAsync(user, 
        [
            nameof(FgsUserRole.Player), 
            nameof(FgsUserRole.Master), 
            nameof(FgsUserRole.Spectator)]);
    }

    public async Task<UserInfoModelWithRoles> GetUserInfoWithRolesAsync(FgsUser user) =>
        new()
        {
            Id = Guid.Parse(user.Id),
            Login = user.UserName ?? string.Empty,
            DisplayName = user.DisplayName,
            Roles = await GetRolesAsync(user)
        };
}

