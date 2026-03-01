using GameServer.Generated;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Health;

/// <summary>
/// Health API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class HealthController : HealthControllerBase
{
    public override Task<ActionResult<Response>> Check(CancellationToken cancellationToken = default)
    {
        var response = new Response { Status = ResponseStatus.Healthy };
        return Task.FromResult<ActionResult<Response>>(Ok(response));
    }
}
