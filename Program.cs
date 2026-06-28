using Microsoft.EntityFrameworkCore;
using MediAgent.Api.Agents;
using MediAgent.Api.Data;
using MediAgent.Api.Endpoints;
using MediAgent.Api.Infrastructure.Ollama;
using MediAgent.Api.Infrastructure.WebSearch;
using MediAgent.Api.Middleware;
using MediAgent.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
if (dbProvider == "Sqlite")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions
                .EnableRetryOnFailure(3)
                .MigrationsAssembly("HealthAgent.Api")
        ));
}

builder.Services.AddSingleton<MediAgentKernel>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IHealthAgentService, HealthAgentService>();
builder.Services.AddScoped<IWebSearchService, WebSearchService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRender", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrWhiteSpace(origin)) return false;
            var uri = new Uri(origin);
            return uri.IsLoopback
                || uri.Host == "localhost"
                || uri.Host.EndsWith(".onrender.com")
                || uri.Host == "healthagent-server.onrender.com";
        })
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowRender");
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapHealthAgentEndpoints();
app.MapConversationEndpoints();
app.MapResourceEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database initialization skipped (SQL Server may not be available)");
    }
}

app.Run();
