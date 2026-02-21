using FGS.Domain.FgsLobby.Entities;
using Microsoft.EntityFrameworkCore;

namespace FGS.DAL.LobbyViewModelRepository;

public class LobbyViewModelContext(DbContextOptions<LobbyViewModelContext> options) : DbContext(options)
{
    public DbSet<LobbyEntity> Lobbies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<LobbyEntity>()
            .ToTable("Lobby")
            .HasKey(k => k.Id);
    }
}