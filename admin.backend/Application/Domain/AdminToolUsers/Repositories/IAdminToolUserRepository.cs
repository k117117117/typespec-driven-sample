using AdminBackend.Domain.AdminToolUsers.Models;

namespace AdminBackend.Domain.AdminToolUsers.Repositories;

/// <summary>
/// 管理ツールユーザーのリポジトリインターフェース。
/// </summary>
public interface IAdminToolUserRepository
{
    Task<IReadOnlyList<AdminToolUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AdminToolUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminToolUser> AddAsync(AdminToolUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(AdminToolUser user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
