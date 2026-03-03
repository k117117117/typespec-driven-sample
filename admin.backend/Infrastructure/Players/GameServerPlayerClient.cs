using System.Net.Http.Json;
using AdminBackend.Domain.Players.Services;

namespace AdminBackend.Infrastructure.Players;

/// <summary>
/// Game Server API を呼び出してプレイヤー情報を取得する HttpClient 実装。
/// </summary>
internal class GameServerPlayerClient(HttpClient httpClient) : IGameServerPlayerClient
{
    public async Task<PagedResult<PlayerDto>> GetAllAsync(int? offset, int? limit, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (offset.HasValue) queryParams.Add($"offset={offset.Value}");
        if (limit.HasValue) queryParams.Add($"limit={limit.Value}");

        var url = queryParams.Count > 0
            ? $"/players?{string.Join("&", queryParams)}"
            : "/players";

        var result = await httpClient.GetFromJsonAsync<GameServerPageResponse>(url, cancellationToken);
        var items = result?.Items?.Select(p => new PlayerDto(p.Id, p.Name, p.Level)).ToList()
            ?? [];
        return new PagedResult<PlayerDto>(items, result?.Total ?? 0);
    }

    public async Task<PlayerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/players/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlayerDto>(cancellationToken);
    }

    private record GameServerPageResponse(List<GameServerPlayerItem> Items, int Total);
    private record GameServerPlayerItem(int Id, string Name, int Level);
}
