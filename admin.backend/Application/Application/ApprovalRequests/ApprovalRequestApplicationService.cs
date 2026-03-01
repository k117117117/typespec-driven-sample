using AdminBackend.Domain.ApprovalRequests.Models;
using AdminBackend.Domain.ApprovalRequests.Repositories;

namespace AdminBackend.Application.ApprovalRequests;

/// <summary>
/// 承認リクエストのアプリケーションサービス。
/// </summary>
public class ApprovalRequestApplicationService(IApprovalRequestRepository repository)
{
    public async Task<IReadOnlyList<ApprovalRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }

    public async Task<ApprovalRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<ApprovalRequest> CreateAsync(string reason, CancellationToken cancellationToken = default)
    {
        var request = ApprovalRequest.Create(reason);
        return await repository.AddAsync(request, cancellationToken);
    }

    public async Task<ApprovalRequest?> UpdateAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        if (request is null) return null;

        request.UpdateReason(reason);
        await repository.UpdateAsync(request, cancellationToken);
        return request;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ApprovalRequest?> ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        if (request is null) return null;

        request.Approve();
        await repository.UpdateAsync(request, cancellationToken);
        return request;
    }

    public async Task<ApprovalRequest?> RejectAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        if (request is null) return null;

        request.Reject();
        await repository.UpdateAsync(request, cancellationToken);
        return request;
    }
}
