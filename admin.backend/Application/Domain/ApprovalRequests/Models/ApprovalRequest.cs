namespace AdminBackend.Domain.ApprovalRequests.Models;

/// <summary>
/// 承認リクエストのドメインモデル。状態遷移のビジネスロジックを持つ。
/// </summary>
public class ApprovalRequest
{
    public int Id { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public ApprovalStatus Status { get; private set; } = ApprovalStatus.Pending;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private ApprovalRequest() { }

    /// <summary>
    /// 新規承認リクエストを作成する。
    /// </summary>
    internal static ApprovalRequest Create(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("理由は必須です。", nameof(reason));

        var now = DateTimeOffset.UtcNow;
        return new ApprovalRequest
        {
            Reason = reason,
            Status = ApprovalStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// 永続化済みデータからドメインモデルを復元する。
    /// </summary>
    internal static ApprovalRequest Reconstruct(int id, string reason, ApprovalStatus status, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        return new ApprovalRequest { Id = id, Reason = reason, Status = status, CreatedAt = createdAt, UpdatedAt = updatedAt };
    }

    /// <summary>
    /// 理由を更新する。
    /// </summary>
    internal void UpdateReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("理由は必須です。", nameof(reason));

        Reason = reason;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// リクエストを承認する。
    /// </summary>
    internal void Approve()
    {
        if (Status != ApprovalStatus.Pending)
            throw new InvalidOperationException($"承認できるのは Pending 状態のリクエストのみです。現在の状態: {Status}");

        Status = ApprovalStatus.Approved;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// リクエストを却下する。
    /// </summary>
    internal void Reject()
    {
        if (Status != ApprovalStatus.Pending)
            throw new InvalidOperationException($"却下できるのは Pending 状態のリクエストのみです。現在の状態: {Status}");

        Status = ApprovalStatus.Rejected;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}
