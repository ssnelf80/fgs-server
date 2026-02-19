using FGS.Auth.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FGS.Auth;

public class AuthContext(DbContextOptions<AuthContext> options) 
    : IdentityDbContext<FgsUser, FgsRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<FgsUser>()
            .Property(u => u.DisplayName)
            .IsRequired();
    }
}