using Microsoft.AspNetCore.Identity;

namespace FGS.Auth.Entities;

public class FgsUser : IdentityUser
{
    public FgsUser() : base()
    {
        
    }

    public FgsUser(string userName) : base(userName)
    {
        DisplayName = userName;
    }

    public string DisplayName { get; set; } = string.Empty;
}