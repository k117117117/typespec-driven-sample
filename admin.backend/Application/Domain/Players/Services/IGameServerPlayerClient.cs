namespace AdminBackend.Domain.Players.Services;

/// <summary>
/// Game Server API 経由でプレイヤー情報を取得するクライアントインターフェース。
/// </summary>
public interface IGameServerPlayerClient
{
    Task<PagedResult<PlayerDto>> GetAllAsync(int? offset, int? limit, CancellationToken cancellationToken = default);
    Task<PlayerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Game Server から取得するプレイヤーの DTO。
/// </summary>
public record PlayerDto(int Id, string Name, int Level);

/// <summary>
/// ページネーション付きの結果。
/// </summary>
public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
