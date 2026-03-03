using AdminBackend.Domain.ApprovalRequests.Models;

namespace AdminBackend.Domain.ApprovalRequests.Repositories;

/// <summary>
/// 承認リクエストのリポジトリインターフェース。
/// </summary>
public interface IApprovalRequestRepository
{
    Task<(IReadOnlyList<ApprovalRequest> Items, int Total)> GetAllAsync(int? offset, int? limit, CancellationToken cancellationToken = default);
    Task<ApprovalRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApprovalRequest> AddAsync(ApprovalRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(ApprovalRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
