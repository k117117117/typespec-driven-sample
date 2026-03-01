using AdminBackend.Domain.ApprovalRequests.Models;
using AdminBackend.Domain.ApprovalRequests.Repositories;
using AdminBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Infrastructure.ApprovalRequests;

/// <summary>
/// EF Core を使用した承認リクエストリポジトリの実装。
/// </summary>
internal class ApprovalRequestRepository(AppDbContext db) : IApprovalRequestRepository
{
    public async Task<IReadOnlyList<ApprovalRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await db.ApprovalRequests.AsNoTracking().ToListAsync(cancellationToken);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<ApprovalRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<ApprovalRequest> AddAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(request);
        db.ApprovalRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return ApprovalRequest.Reconstruct(entity.Id, entity.Reason, ToDomainStatus(entity.Status), entity.CreatedAt, entity.UpdatedAt);
    }

    public async Task UpdateAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([request.Id], cancellationToken)
            ?? throw new InvalidOperationException($"ApprovalRequest {request.Id} not found.");

        entity.Reason = request.Reason;
        entity.Status = ToEntityStatus(request.Status);
        entity.UpdatedAt = request.UpdatedAt;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        if (entity is null) return false;

        db.ApprovalRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ApprovalRequest ToDomain(ApprovalRequestEntity e) =>
        ApprovalRequest.Reconstruct(e.Id, e.Reason, ToDomainStatus(e.Status), e.CreatedAt, e.UpdatedAt);

    private static ApprovalRequestEntity ToEntity(ApprovalRequest r) => new()
    {
        Reason = r.Reason,
        Status = ToEntityStatus(r.Status),
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };

    private static ApprovalStatus ToDomainStatus(ApprovalStatusValue status) => status switch
    {
        ApprovalStatusValue.Approved => ApprovalStatus.Approved,
        ApprovalStatusValue.Rejected => ApprovalStatus.Rejected,
        _ => ApprovalStatus.Pending
    };

    private static ApprovalStatusValue ToEntityStatus(ApprovalStatus status) => status switch
    {
        ApprovalStatus.Approved => ApprovalStatusValue.Approved,
        ApprovalStatus.Rejected => ApprovalStatusValue.Rejected,
        _ => ApprovalStatusValue.Pending
    };
}
