namespace GameServer.Domain.Players.Models;

/// <summary>
/// プレイヤーのドメインモデル。ビジネスロジックをここに集約する。
/// </summary>
public class Player
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Level { get; private set; }

    private Player() { }

    /// <summary>
    /// 新規プレイヤーを作成する。初期レベルは 1。
    /// </summary>
    internal static Player Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("プレイヤー名は必須です。", nameof(name));

        return new Player { Name = name, Level = 1 };
    }

    /// <summary>
    /// 永続化済みデータからドメインモデルを復元する。
    /// </summary>
    public static Player Reconstruct(int id, string name, int level)
    {
        return new Player { Id = id, Name = name, Level = level };
    }

    /// <summary>
    /// プレイヤー情報を更新する。
    /// </summary>
    internal void UpdateProfile(string name, int level)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("プレイヤー名は必須です。", nameof(name));
        if (level < 1)
            throw new ArgumentException("レベルは 1 以上である必要があります。", nameof(level));

        Name = name;
        Level = level;
    }
}
