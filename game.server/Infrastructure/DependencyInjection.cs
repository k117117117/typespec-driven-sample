using GameServer.Domain.Players.Repositories;
using GameServer.Infrastructure.Data;
using GameServer.Infrastructure.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Infrastructure;

/// <summary>
/// Infrastructure 層の DI 登録を行う拡張メソッド。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPlayerRepository, PlayerRepository>();

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
