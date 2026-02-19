using FGS.Auth.Entities;
using FGS.Auth.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FGS.Auth.Managers;

public class FgsRoleManager(
    IRoleStore<FgsRole> store,
    IEnumerable<IRoleValidator<FgsRole>> roleValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    ILogger<RoleManager<FgsRole>> logger)
    : RoleManager<FgsRole>(store, roleValidators, keyNormalizer, errors, logger)
{
    public async Task<IdentityResult> CreateDefaultRolesAsync()
    {
        var result = await CreateAsync(new FgsRole(nameof(FgsUserRoles.Admin)));
        if (!result.Succeeded)
            return result;

        result = await CreateAsync(new FgsRole(nameof(FgsUserRoles.Player)));
        if (!result.Succeeded)
            return result;

        result = await CreateAsync(new FgsRole(nameof(FgsUserRoles.Master)));
        if (!result.Succeeded)
            return result;

        return await CreateAsync(new FgsRole(nameof(FgsUserRoles.Spectator)));
    }
}