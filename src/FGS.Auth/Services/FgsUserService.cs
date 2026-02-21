using FGS.Auth.Entities;
using FGS.Auth.Enums;
using Microsoft.EntityFrameworkCore;

namespace FGS.Auth.Services;

public class FgsUserService(AuthContext db)
{
    public async Task<IReadOnlyCollection<UserEntity>> GetUsersAsync(FgsUserEntitySearchFilter filter,
        CancellationToken cancellationToken)
    {
        return await db.Users
            .Where(filter.GetWhereExpression())
            .Select(user => new UserEntity(
                    Guid.Parse(user.Id), 
                    user.UserName ?? string.Empty,
                    user.DisplayName,
                     db.Roles
                        .Join(db.UserRoles, role => role.Id, userRole => userRole.RoleId,
                            (role, userRole) => new { role, userRole })
                        .Where(link => link.userRole.UserId == user.Id)
                        .Select(link => Enum.Parse<FgsUserRole>(link.role.Name!))
                        .ToList()))
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ToListAsync(cancellationToken);
    }
}