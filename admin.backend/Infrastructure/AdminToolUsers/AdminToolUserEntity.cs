namespace AdminBackend.Infrastructure.AdminToolUsers;

/// <summary>
/// 管理ツールユーザーの EF Core エンティティ。
/// </summary>
internal class AdminToolUserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AdminToolUserRoleValue Role { get; set; } = AdminToolUserRoleValue.Staff;
}

internal enum AdminToolUserRoleValue
{
    Admin,
    Staff
}
