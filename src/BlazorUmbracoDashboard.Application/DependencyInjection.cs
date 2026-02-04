using BlazorUmbracoDashboard.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorUmbracoDashboard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddScoped<ContentService>();
        return services;
    }
}
