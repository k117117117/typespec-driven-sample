using Backend.Data;
using Backend.Generated;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

/// <summary>
/// AdminToolUsers API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class AdminToolUsersController(AppDbContext db) : AdminToolUsersControllerBaseControllerBase
{
    public override async Task<ActionResult<ICollection<ReadAdminToolUserItem>>> List(CancellationToken cancellationToken = default)
    {
        var entities = await db.AdminToolUsers.ToListAsync(cancellationToken);
        ICollection<ReadAdminToolUserItem> result = [.. entities.Select(ToListItem)];

        return Ok(result);
    }

    public override async Task<ActionResult<ReadAdminToolUser>> Read(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.AdminToolUsers.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        return Ok(ToReadDto(entity));
    }

    public override async Task<IActionResult> Create([FromBody] CreateAdminToolUser body, CancellationToken cancellationToken = default)
    {
        var entity = new AdminToolUserEntity
        {
            Name = body.Name,
            Email = body.Email,
            Role = body.Role switch
            {
                CreateAdminToolUserRole.Admin => AdminToolUserRoleValue.Admin,
                _ => AdminToolUserRoleValue.Staff
            }
        };
        db.AdminToolUsers.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(Read), new { id = entity.Id }, ToReadDto(entity));
    }

    public override async Task<IActionResult> Update(int id, [FromBody] UpdateAdminToolUser body, CancellationToken cancellationToken = default)
    {
        var entity = await db.AdminToolUsers.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        entity.Name = body.Name;
        entity.Email = body.Email;
        entity.Role = body.Role switch
        {
            UpdateAdminToolUserRole.Admin => AdminToolUserRoleValue.Admin,
            _ => AdminToolUserRoleValue.Staff
        };
        await db.SaveChangesAsync(cancellationToken);

        return Ok(ToReadDto(entity));
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.AdminToolUsers.FindAsync([id], cancellationToken);
        if (entity == null) return NotFound();

        db.AdminToolUsers.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static ReadAdminToolUserItem ToListItem(AdminToolUserEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Email = e.Email,
        Role = e.Role switch
        {
            AdminToolUserRoleValue.Admin => ReadAdminToolUserItemRole.Admin,
            _ => ReadAdminToolUserItemRole.Staff
        }
    };

    private static ReadAdminToolUser ToReadDto(AdminToolUserEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Email = e.Email,
        Role = e.Role switch
        {
            AdminToolUserRoleValue.Admin => ReadAdminToolUserRole.Admin,
            _ => ReadAdminToolUserRole.Staff
        }
    };
}
