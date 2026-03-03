using GameServer.Domain.Players.Models;
using GameServer.Domain.Players.Repositories;
using GameServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Infrastructure.Players;

/// <summary>
/// EF Core を使用したプレイヤーリポジトリの実装。
/// Entity ↔ ドメインモデルの変換をここで行う。
/// </summary>
internal class PlayerRepository(AppDbContext db) : IPlayerRepository
{
    public async Task<(IReadOnlyList<Player> Items, int Total)> GetAllAsync(int? offset, int? limit, CancellationToken cancellationToken = default)
    {
        var query = db.Players.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);

        if (offset.HasValue)
            query = query.Skip(offset.Value);
        if (limit.HasValue)
            query = query.Take(limit.Value);

        var entities = await query.ToListAsync(cancellationToken);
        return (entities.Select(ToDomain).ToList(), total);
    }

    public async Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Players.FindAsync([id], cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(player);
        db.Players.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Player.Reconstruct(entity.Id, entity.Name, entity.Level);
    }

    public async Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        var entity = await db.Players.FindAsync([player.Id], cancellationToken)
            ?? throw new InvalidOperationException($"Player {player.Id} not found.");

        entity.Name = player.Name;
        entity.Level = player.Level;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Players.FindAsync([id], cancellationToken);
        if (entity is null) return false;

        db.Players.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Player ToDomain(PlayerEntity e) =>
        Player.Reconstruct(e.Id, e.Name, e.Level);

    private static PlayerEntity ToEntity(Player p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Level = p.Level
    };
}
