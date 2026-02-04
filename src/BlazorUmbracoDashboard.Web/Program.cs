using BlazorUmbracoDashboard.Application;
using BlazorUmbracoDashboard.Infrastructure;
using BlazorUmbracoDashboard.Infrastructure.Data;
using BlazorUmbracoDashboard.Web.Components;
using BlazorUmbracoDashboard.Web.Hubs;
using BlazorUmbracoDashboard.Web.Middleware;
using BlazorUmbracoDashboard.Web.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog early for startup logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((context, config) => config
        .ReadFrom.Configuration(context.Configuration));

    // Add Clean Architecture layers
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Blazor
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // SignalR
    builder.Services.AddSignalR();

    // Toast notification service (scoped per circuit)
    builder.Services.AddScoped<ToastService>();

    var app = builder.Build();

    // Auto-migrate and seed database
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db);
    }

    // Global exception handler
    app.UseMiddleware<GlobalExceptionHandler>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    // Serilog request logging
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAntiforgery();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.MapHub<DashboardHub>("/hubs/dashboard");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
