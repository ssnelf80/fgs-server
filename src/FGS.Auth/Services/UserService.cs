using FGS.Auth.Entities;
using FGS.Auth.Enums;
using FGS.Common.SearchFilter;
using Microsoft.EntityFrameworkCore;

namespace FGS.Auth.Services;

public class UserService(AuthContext db)
{
    public async Task<IReadOnlyCollection<UserEntity>> GetUsersAsync(UserEntitySearchFilter filter,
        CancellationToken cancellationToken)
    {
        // todo в метод фильтра или в страшную рефлексию
        PredicateBuilder<FgsUser> predicate = new();
        if (!string.IsNullOrWhiteSpace(filter.SearchString))
        {
            predicate.Or(x => x.Id == filter.SearchString);
            if (filter.IgnoreCase)
            {
                predicate.Or(x 
                    => x.UserName != null && x.UserName.ToLower().Contains(filter.GetLowercaseSearchString()));
                predicate.Or(x 
                    => x.DisplayName.Contains(filter.GetLowercaseSearchString()));
            }
            else
            {
                predicate.Or(x => x.UserName != null && x.UserName.Contains(filter.SearchString));
                predicate.Or(x => x.DisplayName.Contains(filter.SearchString));
            }
        }

        return await db.Users
            .Where(predicate.GetLambda())
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