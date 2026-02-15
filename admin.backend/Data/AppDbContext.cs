using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
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

public class ApprovalRequestEntity
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ApprovalStatusValue Status { get; set; } = ApprovalStatusValue.Pending;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public enum ApprovalStatusValue
{
    Pending,
    Approved,
    Rejected
}

public class AdminToolUserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AdminToolUserRoleValue Role { get; set; } = AdminToolUserRoleValue.Staff;
}

public enum AdminToolUserRoleValue
{
    Admin,
    Staff
}
