using FGS.DAL.ViewModel.Entities;
using FGS.Domain.FgsLobby.Entities;
using Microsoft.EntityFrameworkCore;

namespace FGS.DAL.Context;

public class FgsViewModelContext(DbContextOptions<FgsViewModelContext> options) : DbContext(options)
{
    public DbSet<LobbyEntity> Lobbies { get; set; }
    public DbSet<EventSourceStreamTracker>  StreamTrackers { get; set; }
    public DbSet<ConnectionTracker> UserConnections { get; set; }

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
        
        builder.Entity<EventSourceStreamTracker>()
            .ToTable("EventSourceStreamTracker")
            .HasKey(k => k.StreamTypeId);

        builder.Entity<ConnectionTracker>()
            .ToTable("ConnectionTracker")
            .HasKey(x => x.UserId);

        builder.Entity<ConnectionTracker>()
            .HasIndex(x => x.LobbyId);
    }
}