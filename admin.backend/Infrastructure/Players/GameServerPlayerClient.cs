using System.Net.Http.Json;
using AdminBackend.Domain.Players.Services;

namespace AdminBackend.Infrastructure.Players;

/// <summary>
/// Game Server API を呼び出してプレイヤー情報を取得する HttpClient 実装。
/// </summary>
public class GameServerPlayerClient(HttpClient httpClient) : IGameServerPlayerClient
{
    public async Task<IReadOnlyList<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var players = await httpClient.GetFromJsonAsync<List<PlayerDto>>("/players", cancellationToken);
        return players ?? [];
    }

    public async Task<PlayerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/players/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlayerDto>(cancellationToken);
    }
}
