using Microsoft.AspNetCore.Identity;

namespace FGS.Auth.Entities;

public class FgsRole : IdentityRole
{
    public FgsRole() : base()
    {
    }

    public FgsRole(string roleName) : base(roleName)
    {
        
    }
}