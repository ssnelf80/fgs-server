using FGS.DAL.ViewModel.Entities;
using FGS.Domain.FgsLobby.Entities;
using Microsoft.EntityFrameworkCore;

namespace FGS.DAL.Context;

public class FgsViewModelContext(DbContextOptions<FgsViewModelContext> options) : DbContext(options)
{
    public DbSet<LobbyEntity> Lobbies { get; set; }
    public DbSet<EventSourceStreamTracker>  StreamTrackers { get; set; }
    public DbSet<ConnectionTrackerEntity> UserConnections { get; set; }

    public FgsViewModelContext CreateDbContext() => new (options);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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

        builder.Entity<ConnectionTrackerEntity>()
            .ToTable("ConnectionTracker")
            .HasKey(x => x.UserId);

        builder.Entity<ConnectionTrackerEntity>()
            .HasIndex(x => x.LobbyId);
    }
}