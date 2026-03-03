using AdminBackend.Application.AdminToolUsers;
using AdminBackend.Application.ApprovalRequests;
using AdminBackend.Application.Players;
using AdminBackend.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<AdminToolUserApplicationService>();
builder.Services.AddScoped<ApprovalRequestApplicationService>();
builder.Services.AddScoped<PlayerQueryApplicationService>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Port=5432;Database=admin;Username=postgres;Password=postgres",
    builder.Configuration["GameServer:BaseUrl"]
        ?? "http://localhost:5001");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("Content-Type", "Link", "Content-Range", "Accept-Ranges");
        }
    });
});

var app = builder.Build();

// Auto-create database (development only; use Migrations in production)
if (app.Environment.IsDevelopment())
{
    DependencyInjection.MigrateDatabase(app.Services);
}

app.UseExceptionHandler(error => error.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

    var (statusCode, message) = exception switch
    {
        ArgumentException ex => (StatusCodes.Status400BadRequest, ex.Message),
        InvalidOperationException ex => (StatusCodes.Status409Conflict, ex.Message),
        _ => (StatusCodes.Status500InternalServerError, "予期しないエラーが発生しました。")
    };

    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { code = statusCode, message });
}));

app.UseCors();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi.admin.json");
    });
}
app.MapControllers();

app.Run();
