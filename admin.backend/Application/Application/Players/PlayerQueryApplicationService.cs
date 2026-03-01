using AdminBackend.Domain.Players.Services;

namespace AdminBackend.Application.Players;

/// <summary>
/// プレイヤー情報の読み取り専用アプリケーションサービス。
/// Game Server API 経由でデータを取得する。
/// </summary>
public class PlayerQueryApplicationService(IGameServerPlayerClient client)
{
    public async Task<IReadOnlyList<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await client.GetAllAsync(cancellationToken);
    }

    public async Task<PlayerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await client.GetByIdAsync(id, cancellationToken);
    }
}
