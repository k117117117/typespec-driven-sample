namespace GameServer.Infrastructure.Players;

/// <summary>
/// プレイヤーの EF Core エンティティ。永続化用のデータモデル。
/// </summary>
internal class PlayerEntity
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required int Level { get; set; }
}
