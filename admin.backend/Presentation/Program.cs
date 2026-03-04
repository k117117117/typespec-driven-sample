using AdminBackend;
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

if (app.Environment.IsDevelopment())
{
    DependencyInjection.MigrateDatabase(app.Services);
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

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
