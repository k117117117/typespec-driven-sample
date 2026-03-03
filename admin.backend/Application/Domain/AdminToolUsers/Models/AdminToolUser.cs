namespace AdminBackend.Domain.AdminToolUsers.Models;

/// <summary>
/// 管理ツールユーザーのドメインモデル。
/// </summary>
public class AdminToolUser
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public AdminToolUserRole Role { get; private set; } = AdminToolUserRole.Staff;

    private AdminToolUser() { }

    /// <summary>
    /// 新規管理ツールユーザーを作成する。
    /// </summary>
    internal static AdminToolUser Create(string name, string email, AdminToolUserRole role)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("ユーザー名は必須です。", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("メールアドレスは必須です。", nameof(email));

        return new AdminToolUser { Name = name, Email = email, Role = role };
    }

    /// <summary>
    /// 永続化済みデータからドメインモデルを復元する。
    /// </summary>
    internal static AdminToolUser Reconstruct(int id, string name, string email, AdminToolUserRole role)
    {
        return new AdminToolUser { Id = id, Name = name, Email = email, Role = role };
    }

    /// <summary>
    /// ユーザー情報を更新する。
    /// </summary>
    internal void UpdateProfile(string name, string email, AdminToolUserRole role)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("ユーザー名は必須です。", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("メールアドレスは必須です。", nameof(email));

        Name = name;
        Email = email;
        Role = role;
    }
}

public enum AdminToolUserRole
{
    Admin,
    Staff
}
