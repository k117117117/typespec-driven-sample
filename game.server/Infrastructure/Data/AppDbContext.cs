using GameServer.Infrastructure.Players;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Infrastructure.Data;

/// <summary>
/// game.server の EF Core DbContext。
/// </summary>
internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PlayerEntity> Players => Set<PlayerEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
    }
}
