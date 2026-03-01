using GameServer.Domain.Players.Models;
using GameServer.Application.Players;
using GameServer.Generated;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Players;

/// <summary>
/// Players API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class PlayerController(PlayerApplicationService appService) : PlayersControllerBase
{
    public override async Task<ActionResult<ICollection<ReadPlayerItem>>> List(CancellationToken cancellationToken = default)
    {
        var players = await appService.GetAllAsync(cancellationToken);
        ICollection<ReadPlayerItem> result = [.. players.Select(ToListItem)];

        return Ok(result);
    }

    public override async Task<ActionResult<ReadPlayer>> Read(int id, CancellationToken cancellationToken = default)
    {
        var player = await appService.GetByIdAsync(id, cancellationToken);
        if (player is null) return NotFound();

        return Ok(ToReadDto(player));
    }

    public override async Task<IActionResult> Create([FromBody] CreatePlayer body, CancellationToken cancellationToken = default)
    {
        var created = await appService.RegisterAsync(body.Name, cancellationToken);

        return CreatedAtAction(nameof(Read), new { id = created.Id }, ToReadDto(created));
    }

    public override async Task<IActionResult> Update(int id, [FromBody] UpdatePlayer body, CancellationToken cancellationToken = default)
    {
        var updated = await appService.UpdateAsync(id, body.Name, body.Level, cancellationToken);
        if (updated is null) return NotFound();

        return Ok(ToReadDto(updated));
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await appService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();

        return NoContent();
    }

    private static ReadPlayerItem ToListItem(Player p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Level = p.Level
    };

    private static ReadPlayer ToReadDto(Player p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Level = p.Level
    };
}
