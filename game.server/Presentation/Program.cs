using GameServer;
using GameServer.Application.Players;
using GameServer.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<PlayerApplicationService>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=gameserver;Username=postgres;Password=postgres");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    DependencyInjection.MigrateDatabase(app.Services);
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi.json");
    });

    app.MapGet("/openapi.json", async () =>
    {
        var path = Path.Combine(app.Environment.ContentRootPath, "openapi-schema", "openapi.gameserver.json");
        if (!File.Exists(path))
            return Results.NotFound("openapi.json not found. Run 'npm run tsp-and-nswag' to generate it.");

        var json = await File.ReadAllTextAsync(path);
        return Results.Text(json, "application/json");
    });
}
app.MapControllers();

app.Run();
