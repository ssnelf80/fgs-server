using FGS.Adapters.RandomNameGenerator;
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

    public async Task<IReadOnlyDictionary<Guid, string>> GetUserNamesAsync(IReadOnlySet<Guid> userIds,
        CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
            return EmptyUserMap;
        Dictionary<Guid, string> result = [];
        var stringIds = userIds.Select(x => x.ToString()).ToList();
        var realUsers = await db.Users.Where(x => stringIds.Contains(x.Id))
            .Select(x => new { Id = Guid.Parse(x.Id), x.DisplayName })
            .ToListAsync(cancellationToken);
        
        // todo коряво, но работает же)
        var fakeUsers = userIds.Except(realUsers.Select(x => x.Id)).ToList();
        var fakeNames = RandomNameGenerator.GetUniqueNames(fakeUsers.Count);
        HashSet<string> alreadyUsedNames = [];
        foreach (var (fakeUserId, index) in fakeUsers.OrderBy(x => x).Select((x, i) => (x, i)))
        {
            result[fakeUserId] = alreadyUsedNames.Contains(fakeNames[index]) ? fakeNames[index] + $"@{fakeUserId}" : fakeNames[index];
            alreadyUsedNames.Add(fakeNames[index]);
        }

        foreach (var realUser in realUsers)
        {
            result[realUser.Id] = alreadyUsedNames.Contains(realUser.DisplayName) ? realUser.DisplayName + $"@{realUser.Id}" : realUser.DisplayName;
            alreadyUsedNames.Add(realUser.DisplayName);
        }
            
        return result;
    }
    
    private static IReadOnlyDictionary<Guid, string> EmptyUserMap => new Dictionary<Guid, string>();
}