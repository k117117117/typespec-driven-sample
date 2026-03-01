using GameServer.Domain.Players.Models;
using GameServer.Domain.Players.Repositories;

namespace GameServer.Application.Players;

/// <summary>
/// プレイヤーのアプリケーションサービス。ユースケースの調整を行う。
/// </summary>
public class PlayerApplicationService(IPlayerRepository repository)
{
    public async Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }

    public async Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Player> RegisterAsync(string name, CancellationToken cancellationToken = default)
    {
        var player = Player.Create(name);
        return await repository.AddAsync(player, cancellationToken);
    }

    public async Task<Player?> UpdateAsync(int id, string name, int level, CancellationToken cancellationToken = default)
    {
        var player = await repository.GetByIdAsync(id, cancellationToken);
        if (player is null) return null;

        player.UpdateProfile(name, level);
        await repository.UpdateAsync(player, cancellationToken);
        return player;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }
}
