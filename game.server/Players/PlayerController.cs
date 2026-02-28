using GameServer.Generated;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Players;

public class PlayerController : PlayersControllerBaseControllerBase
{
    public override Task<ActionResult<ICollection<ReadPlayerItem>>> List(CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> Create(CreatePlayer body, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ActionResult<ReadPlayer>> Read(int id, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> Update(int id, UpdatePlayer body, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }
}
