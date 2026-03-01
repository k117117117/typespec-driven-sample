using AdminBackend.Domain.AdminToolUsers.Models;
using AdminBackend.Application.AdminToolUsers;
using AdminBackend.Generated;
using Microsoft.AspNetCore.Mvc;

namespace AdminBackend.AdminToolUsers;

/// <summary>
/// AdminToolUsers API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class AdminToolUsersController(AdminToolUserApplicationService appService) : AdminToolUsersControllerBase
{
    public override async Task<ActionResult<ICollection<ReadAdminToolUserItem>>> List(CancellationToken cancellationToken = default)
    {
        var users = await appService.GetAllAsync(cancellationToken);
        ICollection<ReadAdminToolUserItem> result = [.. users.Select(ToListItem)];

        return Ok(result);
    }

    public override async Task<ActionResult<ReadAdminToolUser>> Read(int id, CancellationToken cancellationToken = default)
    {
        var user = await appService.GetByIdAsync(id, cancellationToken);
        if (user is null) return NotFound();

        return Ok(ToReadDto(user));
    }

    public override async Task<IActionResult> Create([FromBody] CreateAdminToolUser body, CancellationToken cancellationToken = default)
    {
        var role = body.Role switch
        {
            CreateAdminToolUserRole.Admin => AdminToolUserRole.Admin,
            _ => AdminToolUserRole.Staff
        };
        var created = await appService.RegisterAsync(body.Name, body.Email, role, cancellationToken);

        return CreatedAtAction(nameof(Read), new { id = created.Id }, ToReadDto(created));
    }

    public override async Task<IActionResult> Update(int id, [FromBody] UpdateAdminToolUser body, CancellationToken cancellationToken = default)
    {
        var role = body.Role switch
        {
            UpdateAdminToolUserRole.Admin => AdminToolUserRole.Admin,
            _ => AdminToolUserRole.Staff
        };
        var updated = await appService.UpdateAsync(id, body.Name, body.Email, role, cancellationToken);
        if (updated is null) return NotFound();

        return Ok(ToReadDto(updated));
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await appService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();

        return NoContent();
    }

    private static ReadAdminToolUserItem ToListItem(AdminToolUser u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Role = u.Role switch
        {
            AdminToolUserRole.Admin => ReadAdminToolUserItemRole.Admin,
            _ => ReadAdminToolUserItemRole.Staff
        }
    };

    private static ReadAdminToolUser ToReadDto(AdminToolUser u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Role = u.Role switch
        {
            AdminToolUserRole.Admin => ReadAdminToolUserRole.Admin,
            _ => ReadAdminToolUserRole.Staff
        }
    };
}
