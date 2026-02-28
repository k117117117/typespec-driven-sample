using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/openapi.json");
});
app.MapControllers();

app.MapGet("/openapi.json", async () =>
{
    // TypeSpec から生成された OpenAPI スキーマ (docker-compose でマウント)
    var path = Path.Combine(app.Environment.ContentRootPath, "schema", "openapi.gameserver.json");
    if (!File.Exists(path))
        return Results.NotFound("openapi.json not found. Run 'npm run tsp-and-nswag' to generate it.");

    var json = await File.ReadAllTextAsync(path);
    return Results.Text(json, "application/json");
});

app.Run();
