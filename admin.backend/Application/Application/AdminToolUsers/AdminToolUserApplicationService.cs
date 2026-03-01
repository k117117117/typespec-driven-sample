using AdminBackend.Domain.AdminToolUsers.Models;
using AdminBackend.Domain.AdminToolUsers.Repositories;

namespace AdminBackend.Application.AdminToolUsers;

/// <summary>
/// 管理ツールユーザーのアプリケーションサービス。
/// </summary>
public class AdminToolUserApplicationService(IAdminToolUserRepository repository)
{
    public async Task<IReadOnlyList<AdminToolUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }

    public async Task<AdminToolUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<AdminToolUser> RegisterAsync(string name, string email, AdminToolUserRole role, CancellationToken cancellationToken = default)
    {
        var user = AdminToolUser.Create(name, email, role);
        return await repository.AddAsync(user, cancellationToken);
    }

    public async Task<AdminToolUser?> UpdateAsync(int id, string name, string email, AdminToolUserRole role, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);
        if (user is null) return null;

        user.UpdateProfile(name, email, role);
        await repository.UpdateAsync(user, cancellationToken);
        return user;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }
}
