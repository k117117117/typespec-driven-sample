using GameServer.Domain.Players.Models;

namespace GameServer.Domain.Players.Repositories;

/// <summary>
/// プレイヤー集約のリポジトリインターフェース。
/// </summary>
public interface IPlayerRepository
{
    Task<(IReadOnlyList<Player> Items, int Total)> GetAllAsync(int? offset, int? limit, CancellationToken cancellationToken = default);
    Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default);
    Task UpdateAsync(Player player, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
