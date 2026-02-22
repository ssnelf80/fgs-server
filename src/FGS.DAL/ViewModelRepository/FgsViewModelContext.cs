using FGS.DAL.ViewModelRepository.Entities;
using FGS.Domain.FgsLobby.Entities;
using Microsoft.EntityFrameworkCore;

namespace FGS.DAL.ViewModelRepository;

public class FgsViewModelContext(DbContextOptions<FgsViewModelContext> options) : DbContext(options)
{
    public DbSet<LobbyEntity> Lobbies { get; set; }
    public DbSet<EventSourceStreamTracker>  StreamTrackers { get; set; }

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
    }
}