using FGS.Auth;
using FGS.Auth.Entities;
using FGS.Auth.Enums;
using FGS.Auth.Managers;
using FGS.Auth.Services;
using FGS.DAL.LobbyViewModelRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FGS.Host.Extensions;

public static class ServiceExtensions
{
    public static IHostApplicationBuilder AddAuthContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("AuthConnection")));
        builder.Services.AddScoped<FgsUserService>();

        builder.Services.AddIdentity<FgsUser, FgsRole>(o =>
            {
                o.Password = new PasswordOptions
                {
                    RequiredLength = 0,
                    RequiredUniqueChars = 0,
                    RequireNonAlphanumeric = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireDigit = false
                };
            })
            .AddUserManager<FgsUserManager>()
            .AddRoleManager<FgsRoleManager>()
            .AddSignInManager<SignInManager<FgsUser>>()
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();
        
        return builder;
    }

    public static IHostApplicationBuilder AddViewModelContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<LobbyViewModelContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("LobbyViewModelConnection")));
        return builder;
    }

    public static async Task ViewModelConfigureAsync(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<LobbyViewModelContext>();
            await context.Database.MigrateAsync();
                
            logger.LogInformation("Init LobbyViewModel Database");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the auth database");
        }
    }

    public static async Task AuthConfigureAsync(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AuthContext>();
            await context.Database.MigrateAsync();

            if (await context.Roles.AnyAsync() || await context.Users.AnyAsync())
                return;
                
            logger.LogInformation("Init Auth Database");

            var roleManager = services.GetRequiredService<FgsRoleManager>();
            await roleManager.CreateDefaultRolesAsync();
           
            var userManager = services.GetRequiredService<FgsUserManager>();
            await userManager.CreateDefaultAdminAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the auth database");
        }
    }
}