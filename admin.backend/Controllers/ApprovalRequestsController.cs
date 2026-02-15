using Backend.Data;
using Backend.Generated;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

/// <summary>
/// ApprovalRequests API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class ApprovalRequestsController(AppDbContext db) : ApprovalRequestsControllerBaseControllerBase
{
    public override async Task<ActionResult<ICollection<ReadApprovalRequestItem>>> List(CancellationToken cancellationToken = default)
    {
        var entities = await db.ApprovalRequests.ToListAsync(cancellationToken);
        ICollection<ReadApprovalRequestItem> result = [.. entities.Select(ToListItem)];

        return Ok(result);
    }

    public override async Task<ActionResult<ReadApprovalRequest>> Read(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        return Ok(ToReadDto(entity));
    }

    public override async Task<IActionResult> Create([FromBody] CreateApprovalRequest body, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var entity = new ApprovalRequestEntity
        {
            Reason = body.Reason,
            Status = ApprovalStatusValue.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.ApprovalRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(Read), new { id = entity.Id }, ToReadDto(entity));
    }

    public override async Task<IActionResult> Update(int id, [FromBody] UpdateApprovalRequest body, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        entity.Reason = body.Reason;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Ok(ToReadDto(entity));
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        db.ApprovalRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> Approve(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        entity.Status = ApprovalStatusValue.Approved;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> Reject(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ApprovalRequests.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        entity.Status = ApprovalStatusValue.Rejected;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    private static ReadApprovalRequestItem ToListItem(ApprovalRequestEntity e) => new()
    {
        Id = e.Id,
        Reason = e.Reason,
        Status = e.Status switch
        {
            ApprovalStatusValue.Approved => ReadApprovalRequestItemStatus.Approved,
            ApprovalStatusValue.Rejected => ReadApprovalRequestItemStatus.Rejected,
            _ => ReadApprovalRequestItemStatus.Pending
        },
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    private static ReadApprovalRequest ToReadDto(ApprovalRequestEntity e) => new()
    {
        Id = e.Id,
        Reason = e.Reason,
        Status = e.Status switch
        {
            ApprovalStatusValue.Approved => ReadApprovalRequestStatus.Approved,
            ApprovalStatusValue.Rejected => ReadApprovalRequestStatus.Rejected,
            _ => ReadApprovalRequestStatus.Pending
        },
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
