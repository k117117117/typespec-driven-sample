using AdminBackend.Domain.ApprovalRequests.Models;
using AdminBackend.Application.ApprovalRequests;
using AdminBackend.Generated;
using Microsoft.AspNetCore.Mvc;

namespace AdminBackend.ApprovalRequests;

/// <summary>
/// ApprovalRequests API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class ApprovalRequestsController(ApprovalRequestApplicationService appService) : ApprovalRequestsControllerBase
{
    public override async Task<ActionResult<Response2>> List(int? offset, int? limit, CancellationToken cancellationToken = default)
    {
        var (requests, total) = await appService.GetAllAsync(offset, limit, cancellationToken);

        return Ok(new Response2
        {
            Items = [.. requests.Select(ToListItem)],
            Total = total
        });
    }

    public override async Task<ActionResult<ReadApprovalRequest>> Read(int id, CancellationToken cancellationToken = default)
    {
        var request = await appService.GetByIdAsync(id, cancellationToken);
        if (request is null) return NotFound();

        return Ok(ToReadDto(request));
    }

    public override async Task<IActionResult> Create([FromBody] CreateApprovalRequest body, CancellationToken cancellationToken = default)
    {
        var created = await appService.CreateAsync(body.Reason, cancellationToken);

        return CreatedAtAction(nameof(Read), new { id = created.Id }, ToReadDto(created));
    }

    public override async Task<IActionResult> Update(int id, [FromBody] UpdateApprovalRequest body, CancellationToken cancellationToken = default)
    {
        var updated = await appService.UpdateAsync(id, body.Reason, cancellationToken);
        if (updated is null) return NotFound();

        return Ok(ToReadDto(updated));
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await appService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();

        return NoContent();
    }

    public override async Task<IActionResult> Approve(int id, CancellationToken cancellationToken = default)
    {
        var approved = await appService.ApproveAsync(id, cancellationToken);
        if (approved is null) return NotFound();

        return Ok();
    }

    public override async Task<IActionResult> Reject(int id, CancellationToken cancellationToken = default)
    {
        var rejected = await appService.RejectAsync(id, cancellationToken);
        if (rejected is null) return NotFound();

        return Ok();
    }

    private static ReadApprovalRequestItem ToListItem(ApprovalRequest r) => new()
    {
        Id = r.Id,
        Reason = r.Reason,
        Status = r.Status switch
        {
            ApprovalStatus.Approved => ReadApprovalRequestItemStatus.Approved,
            ApprovalStatus.Rejected => ReadApprovalRequestItemStatus.Rejected,
            ApprovalStatus.Pending => ReadApprovalRequestItemStatus.Pending,
            _ => throw new ArgumentException($"不明なステータス: {r.Status}")
        },
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };

    private static ReadApprovalRequest ToReadDto(ApprovalRequest r) => new()
    {
        Id = r.Id,
        Reason = r.Reason,
        Status = r.Status switch
        {
            ApprovalStatus.Approved => ReadApprovalRequestStatus.Approved,
            ApprovalStatus.Rejected => ReadApprovalRequestStatus.Rejected,
            ApprovalStatus.Pending => ReadApprovalRequestStatus.Pending,
            _ => throw new ArgumentException($"不明なステータス: {r.Status}")
        },
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
