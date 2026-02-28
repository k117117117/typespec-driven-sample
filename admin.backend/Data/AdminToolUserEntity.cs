namespace Backend.Data;

/// <summary>
/// 管理ツールユーザーのエンティティ。
/// TypeSpec admin/model.tsp の AdminToolUser モデルに対応。
/// </summary>
public class AdminToolUserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AdminToolUserRoleValue Role { get; set; } = AdminToolUserRoleValue.Staff;
}

public enum AdminToolUserRoleValue
{
    Admin,
    Staff
}
