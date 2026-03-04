using AdminBackend.Domain.Players.Services;
using AdminBackend.Generated.GameServer;

namespace AdminBackend.Infrastructure.Players;

/// <summary>
/// NSwag 生成クライアントを使って Game Server API からプレイヤー情報を取得する実装。
/// </summary>
public class GameServerPlayerClient(PlayersClient playersClient) : IGameServerPlayerClient
{
    public async Task<IReadOnlyList<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var players = await playersClient.ListAsync(cancellationToken);
        return players.Select(p => new PlayerDto(p.Id, p.Name, p.Level)).ToList();
    }

    public async Task<PlayerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await playersClient.ReadAsync(id, cancellationToken);
            return new PlayerDto(player.Id, player.Name, player.Level);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }
}
