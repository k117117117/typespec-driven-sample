namespace AdminBackend.Domain.Players.Services;

/// <summary>
/// Game Server API 経由でプレイヤー情報を取得するクライアントインターフェース。
/// </summary>
public interface IGameServerPlayerClient
{
    Task<IReadOnlyList<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PlayerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Game Server から取得するプレイヤーの DTO。
/// </summary>
public record PlayerDto(int Id, string Name, int Level);
