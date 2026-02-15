using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Port=5432;Database=admin;Username=admin;Password=admin"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Type", "Link", "Content-Range", "Accept-Ranges");
    });
});

var app = builder.Build();

// Auto-create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.MapControllers();

app.MapGet("/openapi.json", async () =>
{
    // TypeSpec から生成された OpenAPI スキーマ (docker-compose でマウント)
    var path = Path.Combine(app.Environment.ContentRootPath, "schema", "openapi.json");
    if (!File.Exists(path))
        return Results.NotFound("openapi.json not found. Run 'npm run tsp-and-nswag' to generate it.");

    var json = await File.ReadAllTextAsync(path);
    return Results.Text(json, "application/json");
});

app.MapGet("/healthz", () => Results.Ok("Healthy"));

app.Run();
