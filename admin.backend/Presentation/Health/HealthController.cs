using AdminBackend.Generated;
using Microsoft.AspNetCore.Mvc;

namespace AdminBackend.Health;

/// <summary>
/// Health API の実装。NSwag が生成した抽象コントローラーを継承。
/// </summary>
public class HealthController : HealthControllerBase
{
    public override Task<ActionResult<Response3>> Check(CancellationToken cancellationToken = default)
    {
        var response = new Response3 { Status = Response3Status.Healthy };
        return Task.FromResult<ActionResult<Response3>>(Ok(response));
    }
}
