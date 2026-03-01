using AdminBackend.Infrastructure.AdminToolUsers;
using AdminBackend.Infrastructure.ApprovalRequests;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Infrastructure.Data;

/// <summary>
/// admin.backend の EF Core DbContext。
/// </summary>
internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApprovalRequestEntity> ApprovalRequests => Set<ApprovalRequestEntity>();
    public DbSet<AdminToolUserEntity> AdminToolUsers => Set<AdminToolUserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalRequestEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<AdminToolUserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Role).HasConversion<string>();
        });
    }
}
