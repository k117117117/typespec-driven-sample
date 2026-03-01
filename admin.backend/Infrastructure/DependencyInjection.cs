using AdminBackend.Domain.AdminToolUsers.Repositories;
using AdminBackend.Domain.ApprovalRequests.Repositories;
using AdminBackend.Infrastructure.AdminToolUsers;
using AdminBackend.Infrastructure.ApprovalRequests;
using AdminBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdminBackend.Infrastructure;

/// <summary>
/// Infrastructure 層の DI 登録を行う拡張メソッド。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAdminToolUserRepository, AdminToolUserRepository>();
        services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();

        return services;
    }

    /// <summary>
    /// データベースの自動作成（開発環境のみ）。
    /// </summary>
    public static void EnsureDatabaseCreated(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}
