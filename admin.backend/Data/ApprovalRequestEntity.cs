namespace Backend.Data;

/// <summary>
/// 承認リクエストのエンティティ。
/// TypeSpec admin/model.tsp の ApprovalRequest モデルに対応。
/// </summary>
public class ApprovalRequestEntity
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ApprovalStatusValue Status { get; set; } = ApprovalStatusValue.Pending;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public enum ApprovalStatusValue
{
    Pending,
    Approved,
    Rejected
}
