using AdminBackend.Domain.AdminToolUsers.Models;
using AdminBackend.Domain.AdminToolUsers.Repositories;
using AdminBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Infrastructure.AdminToolUsers;

/// <summary>
/// EF Core を使用した管理ツールユーザーリポジトリの実装。
/// </summary>
internal class AdminToolUserRepository(AppDbContext db) : IAdminToolUserRepository
{
    public async Task<IReadOnlyList<AdminToolUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await db.AdminToolUsers.AsNoTracking().ToListAsync(cancellationToken);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<AdminToolUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.AdminToolUsers.FindAsync([id], cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<AdminToolUser> AddAsync(AdminToolUser user, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(user);
        db.AdminToolUsers.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return AdminToolUser.Reconstruct(entity.Id, entity.Name, entity.Email, ToDomainRole(entity.Role));
    }

    public async Task UpdateAsync(AdminToolUser user, CancellationToken cancellationToken = default)
    {
        var entity = await db.AdminToolUsers.FindAsync([user.Id], cancellationToken)
            ?? throw new InvalidOperationException($"AdminToolUser {user.Id} not found.");

        entity.Name = user.Name;
        entity.Email = user.Email;
        entity.Role = ToEntityRole(user.Role);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.AdminToolUsers.FindAsync([id], cancellationToken);
        if (entity is null) return false;

        db.AdminToolUsers.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AdminToolUser ToDomain(AdminToolUserEntity e) =>
        AdminToolUser.Reconstruct(e.Id, e.Name, e.Email, ToDomainRole(e.Role));

    private static AdminToolUserEntity ToEntity(AdminToolUser u) => new()
    {
        Name = u.Name,
        Email = u.Email,
        Role = ToEntityRole(u.Role)
    };

    private static AdminToolUserRole ToDomainRole(AdminToolUserRoleValue role) => role switch
    {
        AdminToolUserRoleValue.Admin => AdminToolUserRole.Admin,
        _ => AdminToolUserRole.Staff
    };

    private static AdminToolUserRoleValue ToEntityRole(AdminToolUserRole role) => role switch
    {
        AdminToolUserRole.Admin => AdminToolUserRoleValue.Admin,
        _ => AdminToolUserRoleValue.Staff
    };
}
