using FGS.Auth.Enums;

namespace FGS.Auth.Entities;

public record UserEntity(
    Guid Id, 
    string Login, 
    string DisplayName, 
    IReadOnlyCollection<FgsUserRole> Roles);
