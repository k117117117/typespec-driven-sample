namespace AdminBackend.Infrastructure.ApprovalRequests;

/// <summary>
/// 承認リクエストの EF Core エンティティ。
/// </summary>
internal class ApprovalRequestEntity
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ApprovalStatusValue Status { get; set; } = ApprovalStatusValue.Pending;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

internal enum ApprovalStatusValue
{
    Pending,
    Approved,
    Rejected
}
