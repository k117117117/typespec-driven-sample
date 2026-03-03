using AdminBackend.Application.Players;
using AdminBackend.Generated;
using Microsoft.AspNetCore.Mvc;

namespace AdminBackend.Players;

/// <summary>
/// Players API の実装。Game Server API 経由でプレイヤー情報を読み取り専用で提供する。
/// </summary>
public class PlayersController(PlayerQueryApplicationService appService) : PlayersControllerBase
{
    public override async Task<ActionResult<Response4>> List(int? offset, int? limit, CancellationToken cancellationToken = default)
    {
        var result = await appService.GetAllAsync(offset, limit, cancellationToken);

        return Ok(new Response4
        {
            Items = [.. result.Items.Select(p => new ReadPlayerItem
            {
                Id = p.Id,
                Name = p.Name,
                Level = p.Level
            })],
            Total = result.Total
        });
    }

    public override async Task<ActionResult<ReadPlayer>> Read(int id, CancellationToken cancellationToken = default)
    {
        var player = await appService.GetByIdAsync(id, cancellationToken);
        if (player is null) return NotFound();

        return Ok(new ReadPlayer
        {
            Id = player.Id,
            Name = player.Name,
            Level = player.Level
        });
    }
}
