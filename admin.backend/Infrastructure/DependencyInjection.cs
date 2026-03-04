using AdminBackend.Domain.AdminToolUsers.Repositories;
using AdminBackend.Domain.ApprovalRequests.Repositories;
using AdminBackend.Domain.Players.Services;
using AdminBackend.Generated.GameServer;
using AdminBackend.Infrastructure.AdminToolUsers;
using AdminBackend.Infrastructure.ApprovalRequests;
using AdminBackend.Infrastructure.Data;
using AdminBackend.Infrastructure.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdminBackend.Infrastructure;

/// <summary>
/// Infrastructure 層の DI 登録を行う拡張メソッド。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, string gameServerBaseUrl)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAdminToolUserRepository, AdminToolUserRepository>();
        services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();

        services.AddHttpClient<PlayersClient>(client =>
        {
            client.BaseAddress = new Uri(gameServerBaseUrl);
        });
        services.AddScoped<IGameServerPlayerClient, GameServerPlayerClient>();

        return services;
    }

    /// <summary>
    /// データベースのマイグレーション適用（開発環境のみ）。
    /// </summary>
    public static void MigrateDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}
